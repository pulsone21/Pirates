using UnityEngine;

public class TerrainChunk
{
    private int size;
    private Vector2 position;
    private Bounds bounds;
    private Vector3 posV3;
    private GameObject MeshObject;


    public TerrainChunk(Vector2 _coordinates, int _size)
    {
        this.size = _size;
        this.position = _coordinates * _size;
        this.bounds = new Bounds(position, Vector3.one * _size);
        this.posV3 = new Vector3(position.x, 0, position.y);

        this.MeshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        this.MeshObject.transform.position = posV3;
        this.MeshObject.transform.localScale = (Vector3.one * _size) / 10f;
    }

    void Update()
    {
        // float viewerDistanceFromNearstEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition))
        // float
    }

    public void SetVisibile(bool visible)
    {
        MeshObject.SetActive(visible);
    }
}