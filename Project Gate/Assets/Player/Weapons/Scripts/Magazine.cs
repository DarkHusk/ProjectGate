using UnityEngine;
using UnityEngine.Events;

public class Magazine : MonoBehaviour
{
    [SerializeField] MagazineData magData;

    GameObject receiver = null;
    GunControl gun;
    int bulletCount = 0;

    void Awake()
    {
        bulletCount = magData.GetMaxBulletCount();
    }

    public void Attach()
    {
        if(receiver != null)
        {
            transform.SetParent(receiver.transform);
            gun = receiver.GetComponentInParent<GunControl>();
        }
    }

    public void Detach()
    {
        if(gun != null)
        {
            gun.DetachMagazine();
            gun = null;
        }
    }

    public Object GetCompatibleGun()
    {
        return magData.GetCompatibleGun();
    }

    public void decrementBulletCout()
    {
        bulletCount--;
        if (bulletCount < 0)
        {
            bulletCount = 0;
        }
    }

    public void IncreaseBulletCount(int n)
    {
        bulletCount += n;
        if (bulletCount > magData.GetMaxBulletCount())
        {
            bulletCount = magData.GetMaxBulletCount();
        }
    }

    public bool isMagEmpty()
    {
        return bulletCount <= 0;
    }

    public Vector3 getLocalPositionOffset()
    {
        return magData.GetLocalPositionOffset();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger && other.gameObject == magData.GetCompatibleGun())
        {
            receiver = other.gameObject;
            transform.SetParent(other.transform);
            transform.localPosition = magData.GetLocalPositionOffset();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger && other.gameObject == magData.GetCompatibleGun())
        {
            receiver = null;
        }
    }
}
