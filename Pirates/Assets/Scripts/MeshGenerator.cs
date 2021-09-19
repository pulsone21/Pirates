using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class MeshGenerator
{

    public static MeshData CreateMeshData(float[,] noiseMap, MapConfig mapConfig, Gradient colorGradient, int levelOfDetail)
    {
        AnimationCurve heightCurve = new AnimationCurve(mapConfig.HeightMultiplayerCurve.keys);
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine, "Terrain");
        int vertexIndex = 0;

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                float curvedHeight = heightCurve.Evaluate(noiseMap[x, y]) * mapConfig.heightMultiplyer;
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, curvedHeight, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;

    }
}
