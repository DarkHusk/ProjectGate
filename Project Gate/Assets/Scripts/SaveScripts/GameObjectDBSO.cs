using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameObjectDBSO", menuName = "Scriptable Objects/GameObjectDBSO")]
public class GameObjectDBSO : ScriptableObject
{
    public List<GameObjectItem> db;
}


[System.Serializable]
public class GameObjectItem 
{
    public int id;
    public GameObject prefab;
}
