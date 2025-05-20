// Made by Marcin "DarkHusk" Przybylek

using UnityEngine;

[CreateAssetMenu(fileName = "MagazineData", menuName = "Scriptable Objects/MagazineData")]
public class MagazineData : ScriptableObject
{
    [SerializeField] int maxBulletCount = 30;
    [SerializeField] Object compatibleGun;
    [SerializeField] Vector3 localPositionOffset;

    public Object GetCompatibleGun()
    {
        return compatibleGun;
    }

    public Vector3 GetLocalPositionOffset()
    {
        return localPositionOffset;
    }

    public int GetMaxBulletCount()
    {
        return maxBulletCount;
    }
}
