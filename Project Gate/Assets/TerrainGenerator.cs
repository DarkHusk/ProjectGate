using System.Runtime.CompilerServices;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public Terrain terrain;
    public float scale = 1;
    public int octaves = 1;
    public float persistance = 0; //decrease in amplitude of octaves
    public float lacunarity = 0; //increse in frequency of octaves

    void Start()
    {
        generateTerrain();
    }

    [ContextMenu("generate")]
    void generateTerrain()
    {
        int res = this.terrain.terrainData.heightmapResolution;
        NoiseMap map = new NoiseMap();
        map.GenerateNoiseMap(res, res, scale, octaves, lacunarity, persistance);
        this.terrain.terrainData.SetHeights(0, 0, map.getNoiseMap());
    }

    void Update()
    {
        
    }
}
