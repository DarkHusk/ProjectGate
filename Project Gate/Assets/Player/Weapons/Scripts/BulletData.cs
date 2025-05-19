using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
public class BulletData : ScriptableObject
{
    [SerializeField] float damage = 1;

    public float GetDamage()
    {
        return damage;
    }
}
