using UnityEngine;
public static class FalloffMapGenerator
{

    public static float[,] GenerateFalloff(int ShunkSize)
    {
        float[,] map = new float[ShunkSize, ShunkSize];

        for (int i = 0; i < ShunkSize; i++)
        {
            for (int j = 0; j < ShunkSize; j++)
            {
                float x = (i / (float)ShunkSize) * 2 - 1;
                float y = (j / (float)ShunkSize) * 2 - 1;

                float value = EvaluateFallOfCurve(Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)));
                map[i, j] = value;
            }
        }
        return map;
    }


    private static float EvaluateFallOfCurve(float value)
    {
        int a = 3;
        float powedA = Mathf.Pow(value, a);
        float b = 5f;

        return powedA / (powedA + Mathf.Pow(b - b * value, a));
    }
}