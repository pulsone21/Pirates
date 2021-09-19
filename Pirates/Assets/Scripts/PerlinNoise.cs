using UnityEngine;
using System;

public static class PerlinNoise
{
    public enum NormalizeMode { Local, Global }

    public static float[,] GenerateNoiseMap(MapConfig mapConfig, Vector2 center, NormalizeMode normalizeMode)
    {
        int mapWidth = mapConfig.chunkSize;
        int mapHeight = mapConfig.chunkSize;
        float[,] noiseMap = new float[mapWidth, mapHeight];

        float scale = mapConfig.perlinNoiseScale;
        int octaves = mapConfig.octaves;
        float persistance = mapConfig.perlinNoisePersistance;
        float lacunarity = mapConfig.perlinNoiseLacunarity;
        Vector2 offset = mapConfig.perlinNoiseOffsett + center;

        System.Random rndSys = new System.Random(mapConfig.seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = mapConfig.initialAmplitude;


        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rndSys.Next(-100000, 100000) + offset.x;
            float offsetY = rndSys.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0) scale = 0.00001f;

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;


        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                amplitude = mapConfig.initialAmplitude;
                float frequency = mapConfig.initialFrequenzy;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (float)(x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (float)(y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                switch (normalizeMode)
                {
                    case NormalizeMode.Local:
                        //This makes the noise mapRange from -1 -> 1 to 0 -> 1;
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                        break;
                    case NormalizeMode.Global:
                        float normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight / mapConfig.MaxHeight);
                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                        break;
                    default:
                        throw new NotImplementedException();
                }
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
        float[,] fallofMap = FalloffMapGenerator.GenerateFalloff(mapConfig.chunkSize);

        for (int x = 0; x < mapConfig.chunkSize; x++)
        {
            for (int y = 0; y < mapConfig.chunkSize; y++)
            {
                // Debug.Log($"FallOfMap Value: {fallofMap[x, y]}, noiseMap Value: {noiseMap[x, y]}, Result = {Mathf.Clamp01(noiseMap[x, y] - fallofMap[x, y])}");
                noiseMap[x, y] = Mathf.Clamp01((noiseMap[x, y] - fallofMap[x, y]));
            }
        }
        return noiseMap;
    }



}
