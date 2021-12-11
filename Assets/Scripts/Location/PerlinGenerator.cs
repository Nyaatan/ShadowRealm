using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGenerator
{
    public static List<int> Generate(GeneratorData data, Vector2 size, int maxOffset, int minOffset, int dimension, int xStart)
    {
        List<float> noiseValues = new List<float>();

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        if (dimension == 1)
        {
            for (int i = 0; i < size.x; ++i)
            {
                float noiseHeight = 0;
                float frequency = 1;
                float amplitude = 1;

                for (int j = 0; j < data.octaves; ++j)
                {

                    float perlinValue = Mathf.PerlinNoise(i / data.scale * frequency, data.alifConstant);

                    noiseHeight += (perlinValue * 2 - 1) * amplitude;
                    amplitude *= data.persistance;
                    frequency *= data.lacunarity;

                }
                noiseValues.Add(noiseHeight);

                maxNoiseHeight = Mathf.Max(maxNoiseHeight, noiseHeight);
                minNoiseHeight = Mathf.Min(minNoiseHeight, noiseHeight);
            }
        } else
        {
            for (int i = 0; i < size.y; ++i)
            {
                float noiseHeight = 0;
                float frequency = 1;
                float amplitude = 1;
                for (int j = 0; j < data.octaves; ++j)
                {

                    float perlinValue = Mathf.PerlinNoise(data.alifConstant, i / data.scale * frequency);

                    noiseHeight += (Mathf.Min(Mathf.Max(perlinValue, 0f), 1f)) * amplitude;
                    amplitude *= data.persistance;
                    frequency *= data.lacunarity;

                }
                noiseValues.Add(noiseHeight * data.scale);

                maxNoiseHeight = Mathf.Max(maxNoiseHeight, noiseHeight);
                minNoiseHeight = Mathf.Min(minNoiseHeight, noiseHeight);

            }
        }

        List<int> result = new List<int>();

        foreach (float val in noiseValues)
        {
            result.Add((int)((Mathf.InverseLerp(0, maxNoiseHeight + Mathf.Abs(minNoiseHeight), val + Mathf.Abs(minNoiseHeight))) * (maxOffset - minOffset) + minOffset));
            //Debug.Log((Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, val) + minNoiseHeight));
            // Debug.Log(result[result.Count-1]);
        }

        data.alifConstant += Random.Range(0f, 1f);

        return result;
    }

    public static float[][] Generate2D(GeneratorData data, float xStart, float yStart, float xSize, float ySize, float factor)
    {
        List<List<float>> noiseValues = new List<List<float>>();
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int j = 0; j < ySize; ++j)
        {
            noiseValues.Add(new List<float>());
            for (int i = 0; i < xSize; ++i)
            {
                float noiseHeight = 0;
                float frequency = 1;
                float amplitude = 1;

                for (int k = 0; k < data.octaves; ++k)
                {

                    float perlinValue = Mathf.PerlinNoise(i / data.scale * frequency / factor, j / data.scale * frequency / factor);

                    noiseHeight += (perlinValue * 2 - 1) * amplitude;
                    amplitude *= data.persistance;
                    frequency *= data.lacunarity;

                }
                noiseValues[j].Add(noiseHeight * data.scale);

                maxNoiseHeight = Mathf.Max(maxNoiseHeight, noiseHeight);
                minNoiseHeight = Mathf.Min(minNoiseHeight, noiseHeight);
            }
        }

        List<float[]> rows = new List<float[]>();

        foreach (List<float> row in noiseValues)
        {
            List<float> clippedRow = new List<float>();
            foreach (float val in row) clippedRow.Add(Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, val));
            rows.Add(row.ToArray());
        }
        return rows.ToArray();

    }
}
