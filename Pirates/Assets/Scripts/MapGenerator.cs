using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MapGenerator : MonoBehaviour
{
    public GenerateTexture generateTexture;
    public MapConfig mapConfig;
    public GenerateTexture.DrawMode drawMode;
    public Material meshMaterial;
    public Material vertexShaderMaterial;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;
    public MapData mapData;

    public PerlinNoise.NormalizeMode normalizeMode;

    private Queue<MapThreadInfo<MapData>> MapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    private Queue<MapThreadInfo<MeshData>> MeshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private void Awake()
    {
        // this.transform.localScale = new Vector3(mapConfig.chunkSize, mapConfig.chunkSize, mapConfig.chunkSize);
    }
    void Start()
    {

        GenerateWorldData(Vector2.zero, normalizeMode);
    }

    MapData GenerateWorldData(Vector2 center, PerlinNoise.NormalizeMode normalizeMode)
    {
        float[,] noiseMap = PerlinNoise.GenerateNoiseMap(mapConfig, center, normalizeMode);
        mapData = new MapData(noiseMap, generateTexture.GenerateTerrainColorMap(noiseMap));
        return mapData;
    }

    public void DrawMapInEditor()
    {
        GenerateWorldData(Vector2.zero, normalizeMode);
        Mesh newMesh = MeshGenerator.CreateMeshData(mapData.heightMap, mapConfig, generateTexture.terrainGradient, mapConfig.editorPreviewLOD).CreateMesh();
        meshFilter.mesh = newMesh;
        meshCollider.sharedMesh = newMesh;

        if (!mapConfig.useColorGradient)
        {
            Texture text = generateTexture.GetTexture2D(mapData.heightMap, drawMode);
            meshMaterial.mainTexture = text;
            meshRenderer.sharedMaterial = meshMaterial;
        }
        else
        {
            meshRenderer.sharedMaterial = vertexShaderMaterial;
        }

        this.transform.localScale = new Vector3(mapConfig.chunkSize, mapConfig.chunkSize, mapConfig.chunkSize);
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };
        new Thread(threadStart).Start();
    }

    private void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateWorldData(center, normalizeMode);
        lock (MapDataThreadInfoQueue)
        {
            MapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int levelOfDetail, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
                {
                    MeshDataThread(mapData, levelOfDetail, callback);
                };
        new Thread(threadStart).Start();
    }

    private void MeshDataThread(MapData mapData, int levelOfDetail, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.CreateMeshData(mapData.heightMap, mapConfig, generateTexture.terrainGradient, levelOfDetail);
        lock (MeshDataThreadInfoQueue)
        {
            MeshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        if (MapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < MapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = MapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (MeshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < MeshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = MeshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.parameter = parameter;
            this.callback = callback;
        }
    }
}
public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] _heightMap, Color[] _colorMap)
    {
        heightMap = _heightMap;
        colorMap = _colorMap;
    }
}
