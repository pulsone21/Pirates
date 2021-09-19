using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Configs/MapConfig", fileName = "New MapConfig")]
public class MapConfig : ScriptableObject
{
    public int chunkSize
    {
        get
        {
            if (useFlatShading)
            {
                return 95;
            }
            else
            {
                return 241;
            }
        }
    }
    public int octaves;
    [Range(1, 6)] public int editorPreviewLOD;
    public bool useFlatShading, useColorGradient;
    public AnimationCurve HeightMultiplayerCurve;
    [Range(0, 25)] public float perlinNoiseScale;
    [Range(1f, 50f)] public float heightMultiplyer;
    [Range(0f, 1f)] public float perlinNoisePersistance;
    [Range(0f, 4f)] public float perlinNoiseLacunarity;
    public Vector2 perlinNoiseOffsett;
    public bool randomizeSeed;
    public int seed;
    [Range(0, 1)] public float initialFrequenzy;
    [Range(0, 10)] public float initialAmplitude;
    public enum MapMode { EndlessMap, Falloffmap };
    public MapMode mapMode;

    void OnEnable()
    {
        ValidateInput();
        CreateSeed();
    }

    private void CreateSeed()
    {
        if (randomizeSeed)
        {
            seed = (int)Random.Range(0, 100000);
        }
    }

    private void ValidateInput()
    {
        if (octaves == 0) octaves = 4;
        if (perlinNoiseScale == 0) perlinNoiseScale = 2.7f;
        if (perlinNoisePersistance == 0) perlinNoisePersistance = 1.2f;
        if (perlinNoiseLacunarity == 0) perlinNoiseLacunarity = 1.6f;
        if (perlinNoiseOffsett.x == 0) perlinNoiseOffsett.x = 1;
        if (perlinNoiseOffsett.y == 0) perlinNoiseOffsett.y = 1;
    }
}
