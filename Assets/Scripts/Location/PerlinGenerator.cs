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
                noiseValues.Add(noiseHeight * data.scale);

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
}
