// Made by Justyna Piotrowska

using Mono.Cecil;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour
{
    public Terrain terrain;
    [SerializeField] List<TerrainGeneratorSettings> settings;
    public float scale = 1;
    public int octaves = 1;
    [Range(0,1)] public float persistance = 0; //decrease in amplitude of octaves, only in range <0,1>
    public float lacunarity = 0; //increse in frequency of octaves
    public int seed = 0;
    public GameObject[] trees;
    public GameObject[] rocks;
    public GameObject portal;
    [SerializeField] GameObject player;
    [SerializeField] int min_range, max_range;

    float max_object_scale = 3f;
    float buffer;
    Bounds terrain_bounds;

    List<Vector2Int> free_cells;

    void Start()
    {
        //Setting up a terrain type
        int setting = Random.Range(0, settings.Count-1);
        scale = settings[setting].scale;
        octaves = settings[setting].octaves;
        persistance = settings[setting].persistance;
        lacunarity = settings[setting].lacunarity;

        seed = Random.Range(-10000, 10000);

        //preparing a buffer and a list to ensure no objects spawn on each other
        buffer = max_object_scale * 1.5f;
        terrain_bounds = terrain.terrainData.bounds;
        free_cells = new();

        generateTerrain();
    }

    [ContextMenu("generate")]
    void generateTerrain()
    {
        int res = this.terrain.terrainData.heightmapResolution;
        NoiseMap map = new();
        map.GenerateNoiseMap(res, res, seed, scale, octaves, lacunarity, persistance);
        this.terrain.terrainData.SetHeights(0, 0, map.getNoiseMap());
        Vector3 object_position = new();
        terrain_bounds = terrain.terrainData.bounds;

        int x_dimension = Mathf.FloorToInt(terrain_bounds.size.x / buffer);
        int z_dimension = Mathf.FloorToInt(terrain_bounds.size.z / buffer);
        for (int x = 0; x < x_dimension; x++)
            for (int z = 0; z < z_dimension; z++)
                free_cells.Add(new Vector2Int(x, z));

        int cell_index = Random.Range(0, (x_dimension * z_dimension));
        Vector2Int cell = free_cells[cell_index];

        int i = cell_index;
        while(i< (cell_index) + (z_dimension+2))
        {
            if (i == cell_index + 2) { i += z_dimension-2; }
            free_cells.RemoveAt(cell_index);
            i++;
        }
        //Shuffle for random spawn
        object_position.x = this.transform.position.x + cell.x * buffer + buffer / 2f;
        object_position.z = this.transform.position.z + cell.y * buffer + buffer / 2f;
        object_position.y = terrain.SampleHeight(object_position)+5f;
        Instantiate(portal, object_position, terrain.transform.rotation);
        portal.transform.localScale = new Vector3(10f, 10f, 10f);
        ShuffleList(free_cells);
        cell = free_cells[0];
        free_cells.RemoveAt(0);
        object_position.x = this.transform.position.x + cell.x * buffer + buffer / 2f;
        object_position.z = this.transform.position.z + cell.y * buffer + buffer / 2f;
        object_position.y = terrain.SampleHeight(object_position);
        player.transform.position = object_position;

        spawnObject(trees, min_range, max_range, false);
        spawnObject(rocks, min_range / 10, max_range / 10, true);

        gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    //Spawning objects, also preventing them to overlap, by using a grid system
    void spawnObject(GameObject[] spawn_objects, int min_range, int max_range, bool allow_rotation)
    {
        int n;
        float scale;
        GameObject obj;

        foreach (GameObject spawn_object in spawn_objects)
        {
            n = Random.Range(min_range, max_range);
            for (int i = 0; i < n && free_cells.Count > 0; i++)
            {
                Vector2Int cell = free_cells[0];
                free_cells.RemoveAt(0);

                float worldX = this.transform.position.x + cell.x * buffer + buffer / 2f;
                float worldZ = this.transform.position.z + cell.y * buffer + buffer / 2f;

                float terrainY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
                Vector3 position = new Vector3(worldX, terrainY, worldZ);

                scale = Random.Range(0.5f, max_object_scale);
                Quaternion rotation = allow_rotation ? Random.rotation : Quaternion.identity;

                obj = Instantiate(spawn_object, position, rotation);
                obj.transform.localScale = Vector3.one * scale;
            }
        }
    }
    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    void Update()
    {
        
    }
}
