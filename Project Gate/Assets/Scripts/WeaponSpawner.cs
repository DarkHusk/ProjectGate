// Made by Justyna Piotrowska
// Modified by Marcin "DarkHusk" Przybylek

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
    public Button spawnAmmo;
    public GameObject spawner;
    int index = 0;

    void Start()
    {
        imageWindow.texture = weapons[0].image;
        previous.onClick.AddListener(previousImage);
        next.onClick.AddListener(nextImage);
        spawnWeapon.onClick.AddListener(spawnWeaponObject);
        spawnAmmo.onClick.AddListener(spawnAmmoObject);
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

    void spawnAmmoObject()
    {
        GameObject ammo = Instantiate(weapons[index].ammo, spawner.transform.position, spawner.transform.rotation);
    }
}

[System.Serializable]
public struct Weapon
{
    public string name;
    public GameObject model;
    public GameObject ammo;
    public Texture2D image;
}