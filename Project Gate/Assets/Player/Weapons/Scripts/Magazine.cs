using UnityEngine;
using UnityEngine.Events;

public class Magazine : MonoBehaviour
{
    [SerializeField] MagazineData magData;

    GameObject receiver = null;
    GunControl gun;
    Rigidbody rb;
    int bulletCount = 0;

    void Awake()
    {
        bulletCount = magData.GetMaxBulletCount();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.gameObject.GetComponent<GunControl>() != null)
        {
            receiver = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger && other.gameObject.GetComponent<GunControl>() != null)
        {
            receiver = null;
        }
    }
    public void Attach()
    {
        if(receiver != null)
        {
            transform.SetParent(receiver.transform);
            transform.localPosition = magData.GetLocalPositionOffset();
            transform.localRotation = Quaternion.identity;
            rb.isKinematic = true;
            rb.useGravity = false;
            gun = receiver.GetComponentInParent<GunControl>();
            gun.AttachMagazine(this);
        }
    }

    public void Detach()
    {
        if(gun != null)
        {
            transform.SetParent(null);
            gun.DetachMagazine();
            rb.isKinematic = false;
            rb.useGravity = true;
            gun = null;
            Debug.Log("Detached Mag");
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
}
