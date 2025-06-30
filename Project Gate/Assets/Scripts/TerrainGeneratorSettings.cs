using UnityEngine;

[CreateAssetMenu(fileName = "TerrainGeneratorSettings", menuName = "Scriptable Objects/TerrainGeneratorSettings")]
public class TerrainGeneratorSettings : ScriptableObject
{
    public float scale = 1;
    public int octaves = 1;
    [Range(0, 1)] public float persistance = 0; //decrease in amplitude of octaves, only in range <0,1>
    public float lacunarity = 0; //increse in frequency of octaves
}
