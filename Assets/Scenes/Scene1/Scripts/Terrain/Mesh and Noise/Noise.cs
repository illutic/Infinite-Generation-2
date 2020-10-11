using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(Attributes attributes, NormalizeMode normalizeMode)
    {
        int size = attributes.size;
        int seed = attributes.seed;
        float scale = attributes.scale;
        int octaves = attributes.octaves;
        float persistance = attributes.persistance;
        float lacunarity = attributes.lacunarity;
        Vector2 offset = attributes.offset;
        float elevation = attributes.elevation;
        

        int mapWidth = size;
        int mapHeight = size;
        float[,] noiseMap = new float[mapWidth, mapHeight];
        //System.Random random = new System.Random(seed);
        //float randomValue = random.Next(-1000, 1000);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            //float offsetX = prng.Next(-1000, 1000) + offset.x;
            //float offsetY = prng.Next(-1000, 1000) - offset.y;
            //octaveOffsets[i] = new Vector2(offsetX, offsetY);
            //offset += offset;
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + offset.x + seed) / scale * frequency;
                    float sampleY = (y - halfHeight + offset.y + seed) / scale * frequency;

                    float perlinValue = Mathf.Abs(Mathf.PerlinNoise(sampleX, sampleY)) * 2 - 1;
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
                if (normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / maxPossibleHeight;
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    noiseMap[x, y] = Mathf.Pow(noiseMap[x, y], elevation);
                }
            }
        }

        return noiseMap;
    }
}

