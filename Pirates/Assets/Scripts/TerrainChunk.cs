using UnityEngine;
public class TerrainChunk
{

    GameObject meshObject;
    Vector2 position;
    Bounds bounds;

    public MapGenerator mapGenerator;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;


    private LODInfo[] detailLevels;
    private LODMesh[] lodMeshes;

    private MapData mapData;
    private bool mapDataReceived;

    private int previousLODIndex = -1;

    public TerrainChunk(Vector2 coord, int size, Transform parent, MapGenerator _mapGenerator, Material material, LODInfo[] detailLevels)
    {
        position = coord * size;
        bounds = new Bounds(position, Vector2.one * size);
        Vector3 positionV3 = new Vector3(position.x, 0, position.y);

        meshObject = new GameObject($"TerrainChunk_{position.x}_{position.y}");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();

        meshObject.transform.position = positionV3;
        meshObject.transform.parent = parent;

        meshRenderer.sharedMaterial = material;

        SetVisible(false);

        this.detailLevels = detailLevels;
        lodMeshes = new LODMesh[detailLevels.Length];

        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].LOD, UpdateTerrainChunk);
        }

        mapGenerator = _mapGenerator;
        mapGenerator.RequestMapData(position, OnMapDataReceived);

    }

    void OnMapDataReceived(MapData mapData)
    {
        this.mapData = mapData;
        mapDataReceived = true;

        Texture2D texture2D = mapGenerator.generateTexture.GetTexture2D(mapData.heightMap, GenerateTexture.DrawMode.Mesh);
        meshRenderer.material.mainTexture = texture2D;
        UpdateTerrainChunk();
    }

    public void UpdateTerrainChunk()
    {
        if (mapDataReceived)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(EndlessTerrain.viewerPosition));
            bool visible = viewerDstFromNearestEdge <= EndlessTerrain.maxViewDst;

            if (visible)
            {
                int lodIndex = 0;
                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibileDstThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }
                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        meshFilter.mesh = lodMesh.mesh;
                        // meshCollider.sharedMesh = lodMesh.mesh;
                        previousLODIndex = lodIndex;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(mapData);
                    }
                }
            }

            SetVisible(visible);
        }
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        public int LOD;
        System.Action updateCallback;

        public LODMesh(int _LOD, System.Action _updateCallBack)
        {
            this.LOD = _LOD;
            this.updateCallback = _updateCallBack;
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            EndlessTerrain.mapGenerator.RequestMeshData(mapData, LOD, OnMeshDataReceived);
        }

        private void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;
            updateCallback();
        }
    }

}
[System.Serializable]
public struct LODInfo
{
    public int LOD;
    public float visibileDstThreshold;

    public LODInfo(int _LOD, float _visibileDstThreshold)
    {
        LOD = _LOD;
        visibileDstThreshold = _visibileDstThreshold;
    }
}
