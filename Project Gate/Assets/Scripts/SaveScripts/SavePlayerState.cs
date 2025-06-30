using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class SavePlayerState : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObjectDBSO itemDB;

    GameObject holster;

    void Start()
    {
        holster = player.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.CompareTag("Holster"))?.gameObject;
        LoadData();
    }

    public void SaveData()
    {
        PlayerData data = new();
        data.weaponPos = new();
        data.rotation = new();
        if(holster.transform.childCount > 0)
        {
            GameObject gun = holster.transform.GetChild(0).gameObject;
            data.holsterWeaponID = gun.GetComponent<GunControl>().prefabID;
            data.weaponPos.x = gun.transform.localPosition.x;
            data.weaponPos.y = gun.transform.localPosition.y;
            data.weaponPos.z = gun.transform.localPosition.z;

            data.rotation.x = gun.transform.localRotation.x;
            data.rotation.y = gun.transform.localRotation.y;
            data.rotation.z = gun.transform.localRotation.z;
            data.rotation.w = gun.transform.localRotation.w;
            GameObject magazine = gun.GetComponentInChildren<Magazine>()?.gameObject;
            if(magazine != null)
            {
                data.weaponReciverMagID = magazine.GetComponent<Magazine>().prefabID;
            }
            else
            {
                data.holsterWeaponID = -1;
            }
        }
        else
        {
            data.holsterWeaponID = -1;
            data.weaponReciverMagID = -1;
        }
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", JsonConvert.SerializeObject(data));
    }

    public void LoadData()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/PlayerData.json")) return;
        PlayerData data = JsonConvert.DeserializeObject<PlayerData>(System.IO.File.ReadAllText(Application.persistentDataPath + "/PlayerData.json"));
        if(data.holsterWeaponID >= 0)
        {
            GameObject gun = Instantiate(itemDB.db.FirstOrDefault(d => d.id == data.holsterWeaponID).prefab);
            gun.transform.SetParent(holster.transform);
            gun.transform.localPosition = new Vector3(data.weaponPos.x, data.weaponPos.y, data.weaponPos.z);
            gun.transform.localRotation = new Quaternion(data.rotation.x, data.rotation.y, data.rotation.z, data.rotation.w);

            Rigidbody gunRB = gun.GetComponent<Rigidbody>();
            gunRB.isKinematic = true;
            gunRB.useGravity = false;

            if(data.weaponReciverMagID >= 0)
            {
                GameObject mag = Instantiate(itemDB.db.FirstOrDefault(d => d.id == data.weaponReciverMagID).prefab);
                Magazine magScript = mag.GetComponent<Magazine>();
                magScript.Attach(gun);
            }
        }
    }

}

[System.Serializable]
public class PlayerData
{
    public int holsterWeaponID;
    public Position weaponPos;
    public Rotation rotation;
    public int weaponReciverMagID;
}

[System.Serializable]
public class Position
{
    public float x, y, z;
}

[System.Serializable]
public class Rotation
{
    public float x, y, z, w;
}
