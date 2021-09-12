using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTexture : MonoBehaviour
{
    public enum DrawMode { Debug, Mesh }
    public List<TerrainTypes> terrainTypes;
    public Color[] colorMap;
    public Gradient terrainGradient;

    void Awake()
    {
        LoadTerrains();
    }

    public Texture2D GetTexture2D(float[,] noiseMap, DrawMode drawMode, MapConfig mapConfig)
    {
        switch (drawMode)
        {
            case DrawMode.Debug:
                colorMap = GenerateDebugColorMap(noiseMap);
                break;
            case DrawMode.Mesh:
                colorMap = GenerateTerrainColorMap(noiseMap);
                break;
            default:
                throw new NotImplementedException();
        }
        Texture2D texture = new Texture2D(noiseMap.GetLength(0), noiseMap.GetLength(1));
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    private Color[] GenerateDebugColorMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colorMap = new Color[(width * height)];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[(y * width + x)] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        return colorMap;
    }

    public Color[] GenerateTerrainColorMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] terrainMap = new Color[(width * height)];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                terrainMap[(y * width + x)] = GetColorFromTerrainType(noiseMap[x, y]);
            }
        }
        return terrainMap;
    }

    private Gradient GenerateGradient()
    {
        Gradient newGradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[terrainTypes.Count];

        for (int i = 0; i < colorKeys.Length; i++)
        {
            colorKeys[i].color = terrainTypes[i].color;
            colorKeys[i].time = terrainTypes[i].height;
        }

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 1f;
        alphaKeys[1].time = 1f;

        newGradient.SetKeys(colorKeys, alphaKeys);
        return newGradient;
    }

    private Color GetColorFromTerrainType(float height)
    {
        Color color = terrainTypes[0].color;
        for (int i = 1; i < terrainTypes.Count; i++)
        {
            if (height >= terrainTypes[i].height)
            {
                color = terrainTypes[i].color;
            }
        }
        return color;
    }

    public void LoadTerrains()
    {
        terrainTypes.Clear();
        TerrainTypes[] terrainArray = Resources.LoadAll<TerrainTypes>("TerrainTypes");
        foreach (TerrainTypes tT in terrainArray)
        {
            terrainTypes.Add(tT);
        }

        SortTerrainList();
        terrainGradient = GenerateGradient();
    }

    private void SortTerrainList()
    {
        List<TerrainTypes> sortedTerrainTypes = terrainTypes;
        for (int i = 0; i < sortedTerrainTypes.Count - 1; i++)
        {
            // traverse i+1 to array length
            for (int j = i + 1; j < sortedTerrainTypes.Count; j++)
            {
                // compare array element with 
                // all next element
                if (sortedTerrainTypes[i].height > sortedTerrainTypes[j].height)
                {
                    TerrainTypes temp = sortedTerrainTypes[i];
                    sortedTerrainTypes[i] = sortedTerrainTypes[j];
                    sortedTerrainTypes[j] = temp;
                }
            }
        }
        terrainTypes = sortedTerrainTypes;
    }

}
