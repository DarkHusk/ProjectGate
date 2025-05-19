// Made by Justyna Piotrowska

using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using NUnit.Framework;
public class NoiseMap
{
    float[,] Map;
    public void GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float lacunarity, float persistance)  
    {
        Assert.IsTrue(mapWidth > 0 && mapHeight > 0 && octaves >=0);
        System.Random offset = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        int offsetMin = -100000, offsetMax = 100000;
        for (int i = 0; i < octaves; i++)
        {
            octaveOffsets[i] = new Vector2(offset.Next(offsetMin, offsetMax), offset.Next(offsetMin, offsetMax));
        }
        Map = new float[mapWidth, mapHeight];
        float amplitude = 1;
        float frequency = 1;
        float noise = 0;
        if (scale <= 0) { scale = 0.000001f; }

        for(int x = 0; x < mapWidth; x++)
        { 
            for(int y = 0; y < mapHeight; y++) 
            {
                noise = 0;
                amplitude = 1;
                frequency = 1;
                for(int i =0; i< octaves; i++)
                {
                    noise += Mathf.PerlinNoise(x / scale * frequency + octaveOffsets[i].x, 
                        y / scale * frequency + octaveOffsets[i].y) * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                Map[x, y] = noise;
            }
        }
    }
    public float[,] getNoiseMap() { return Map; }
    
}
