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
    public GameObject[] trees;
    public GameObject[] rocks;
    [SerializeField]
    int min_range, max_range;
    void Start()
    {
        generateTerrain();
    }

    [ContextMenu("generate")]
    void generateTerrain()
    {
        int res = this.terrain.terrainData.heightmapResolution;
        NoiseMap map = new();
        map.GenerateNoiseMap(res, res, seed, scale, octaves, lacunarity, persistance);
        this.terrain.terrainData.SetHeights(0, 0, map.getNoiseMap());
        spawnObject(trees, min_range, max_range, false);
        spawnObject(rocks, min_range/10, max_range/10, true);
    }
    void spawnObject(GameObject[] spawn_objects, int min_range, int max_range, bool allow_rotation)
    {
        int n;
        Vector3 object_position = new();
        float x_offset, z_offset;
        float scale;
        Bounds terrain_bounds = terrain.terrainData.bounds;
        GameObject obj;
        foreach (GameObject spawn_object in spawn_objects)
        {
            n = Random.Range(min_range, max_range);
            for(int i = 0; i<n; i++)
            {
                x_offset = Random.Range(terrain_bounds.min.x, terrain_bounds.max.x);
                z_offset = Random.Range(terrain_bounds.min.z, terrain_bounds.max.z);
                object_position.x = x_offset;
                object_position.z = z_offset;
                object_position.y = terrain.SampleHeight(object_position);
                scale = Random.Range(0f, 3f);
                obj = Instantiate(spawn_object,object_position,terrain.transform.rotation);
                obj.transform.localScale = new Vector3(scale,scale,scale);
                if (allow_rotation) { obj.transform.rotation = Random.rotation; }
            }
        }
    }
    void Update()
    {
        
    }
}
