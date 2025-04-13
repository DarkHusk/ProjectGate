using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSpawner : MonoBehaviour
{
    public Weapon[] weapons;
    public RawImage imageWindow;
    public Button previous;
    public Button next;
    public Button spawnWeapon;
    public GameObject spawner;
    int index = 0;

    void Start()
    {
        imageWindow.texture = weapons[0].image;
        previous.onClick.AddListener(previousImage);
        next.onClick.AddListener(nextImage);
        spawnWeapon.onClick.AddListener(spawnWeaponObject);
    }
    void previousImage()
    {
        index--;
        if (index < 0) { index = weapons.Length-1; }
        imageWindow.texture = weapons[index].image;
    }
    void nextImage()
    {
        index++;
        if (index >= weapons.Length) { index = 0; }
        imageWindow.texture = weapons[index].image;
    }
    void spawnWeaponObject()
    {
        GameObject weapon = Instantiate(weapons[index].model,spawner.transform.position,spawner.transform.rotation);
    }
}

[System.Serializable]
public struct Weapon
{
    public string name;
    public GameObject model;
    public Texture2D image;
}