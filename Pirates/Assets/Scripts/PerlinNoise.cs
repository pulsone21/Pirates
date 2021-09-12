using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise
{
    public static float[,] GenerateNoiseMap(MapConfig mapConfig)
    {
        int mapWidth = mapConfig.shunkSize;
        int mapHeight = mapConfig.shunkSize;
        float[,] noiseMap = new float[mapWidth, mapHeight];

        float scale = mapConfig.perlinNoiseScale;
        int octaves = mapConfig.octaves;
        float persistance = mapConfig.perlinNoisePersistance;
        float lacunarity = mapConfig.perlinNoiseLacunarity;
        Vector2 offset = mapConfig.perlinNoiseOffsett;

        System.Random rndSys = new System.Random(mapConfig.seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rndSys.Next(-100000, 100000) + offset.x;
            float offsetY = rndSys.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0) scale = 0.00001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;


        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                float amplitude = mapConfig.initialAmplitude;
                float frequency = mapConfig.initialFrequenzy;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (float)((x - halfWidth) / scale) * frequency + octaveOffsets[i].x;
                    float sampleY = (float)((y - halfHeight) / scale) * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        //This makes the noise mapRange from -1 -> 1 to 0 -> 1; 
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        if (mapConfig.mapMode == MapConfig.MapMode.Falloffmap)
        {
            noiseMap = ApplyFallOfMap(noiseMap, mapConfig);
        }
        return noiseMap;
    }


    private static float[,] ApplyFallOfMap(float[,] noiseMap, MapConfig mapConfig)
    {
        float[,] fallofMap = FalloffMapGenerator.GenerateFalloff(mapConfig.shunkSize);

        for (int x = 0; x < mapConfig.shunkSize; x++)
        {
            for (int y = 0; y < mapConfig.shunkSize; y++)
            {
                // Debug.Log($"FallOfMap Value: {fallofMap[x, y]}, noiseMap Value: {noiseMap[x, y]}, Result = {Mathf.Clamp01(noiseMap[x, y] - fallofMap[x, y])}");
                noiseMap[x, y] = Mathf.Clamp01((noiseMap[x, y] - fallofMap[x, y]));
            }
        }
        return noiseMap;
    }



}
