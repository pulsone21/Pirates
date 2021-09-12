using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MapGenerator : MonoBehaviour
{
    public GenerateTexture generateTexture;
    public MapConfig mapConfig;
    private float[,] noiseMap;
    public GenerateTexture.DrawMode drawMode;
    public Material meshMaterial;
    public Material vertexShaderMaterial;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;

    // Start is called before the first frame update
    void Start()
    {
        GenerateWorld();
    }


    public void GenerateWorld()
    {
        this.transform.localScale = new Vector3(mapConfig.shunkSize, mapConfig.shunkSize, mapConfig.shunkSize);
        noiseMap = PerlinNoise.GenerateNoiseMap(mapConfig);

        Mesh newMesh = MeshGenerator.CreateMeshData(noiseMap, mapConfig, generateTexture.terrainGradient).CreateMesh(mapConfig.useColorGradient);
        meshFilter.mesh = newMesh;
        meshCollider.sharedMesh = newMesh;

        if (!mapConfig.useColorGradient)
        {
            Texture text = generateTexture.GetTexture2D(noiseMap, drawMode, mapConfig);
            meshMaterial.mainTexture = text;
            meshRenderer.sharedMaterial = meshMaterial;
        }
        else
        {
            meshRenderer.sharedMaterial = vertexShaderMaterial;
        }

    }
}
