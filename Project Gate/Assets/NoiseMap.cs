using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
public class NoiseMap
{
    float[,] Map;
    public void GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float lacunarity, float persistance)  
    {
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
                    noise += Mathf.PerlinNoise(x / scale * frequency, y / scale * frequency)*amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                Map[x, y] = noise;
            }
        }
    }
    public float[,] getNoiseMap() { return Map; }
    
}
