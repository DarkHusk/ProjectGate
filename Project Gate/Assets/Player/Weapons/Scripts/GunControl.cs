using UnityEngine;

public class GunControl : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPosition;
    [SerializeField] float bulletSpeed;
    [SerializeField] float bulletTimeOut;

    public void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, bulletSpawnPosition.rotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = bullet.transform.forward * bulletSpeed;
        Destroy(bullet, bulletTimeOut);
    }
}
