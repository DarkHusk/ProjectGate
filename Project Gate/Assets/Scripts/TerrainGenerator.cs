using System.Runtime.CompilerServices;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public Terrain terrain;
    public float scale = 1;
    public int octaves = 1;
    [Range(0,1)]
    public float persistance = 0; //decrease in amplitude of octaves, only in range <0,1>
    public float lacunarity = 0; //increse in frequency of octaves
    public int seed = 0;
    void Start()
    {
        generateTerrain();
    }

    [ContextMenu("generate")]
    void generateTerrain()
    {
        int res = this.terrain.terrainData.heightmapResolution;
        NoiseMap map = new NoiseMap();
        map.GenerateNoiseMap(res, res, seed, scale, octaves, lacunarity, persistance);
        this.terrain.terrainData.SetHeights(0, 0, map.getNoiseMap());
    }

    void Update()
    {
        
    }
}
