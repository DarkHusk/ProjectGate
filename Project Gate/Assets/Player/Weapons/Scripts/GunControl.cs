// Made by Marcin "DarkHusk" Przybylek

using UnityEngine;

public class GunControl : MonoBehaviour
{
    [SerializeField] GameObject bulletTipPrefab;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject shellPrefab;
    [SerializeField] Transform bulletSpawnPosition;
    [SerializeField] Transform shellSpawnPosition;
    [SerializeField] float shellEjectSpeed = 0.5f;
    [SerializeField] float shellTimeOut = 4f;
    [SerializeField] float maxShellRotationSpeed = 0.2f;
    [SerializeField] float bulletSpeed;
    [SerializeField] float bulletTimeOut = 8f;
    [SerializeField] float shootCooldown = 0.1f;
    [SerializeField] bool isAutomatic = true;

    Magazine attachedMag = null;
    bool isShooting = false;
    float shotTimer = 0;
    bool isBulletInChamber = false;

    void Update()
    {
        shotTimer += Time.deltaTime;
        Fire();
    }

    public void StartShooting()
    {
        isShooting = true;
        Fire(); // this is to guarantee that at least one shot is fired when trigger is briefly pressed
    }

    public void StopShooting()
    {
        isShooting = false;
    }

    public void AttachMagazine(Magazine mag)
    {
        attachedMag = mag;
    }

    public void DetachMagazine()
    {
        attachedMag = null;
    }

    public void CockGun()
    {
        if(attachedMag != null && !attachedMag.isMagEmpty())
        {
            isBulletInChamber = true;
            attachedMag.decrementBulletCout();
        }
    }

    public void Eject()
    {
        if(isBulletInChamber)
        {
            Eject(bulletPrefab);
        }
    }

    void Fire()
    {
        if (isShooting && shotTimer > shootCooldown)
        {
            if (!ShootBullet() || !isAutomatic)
            {
                StopShooting();
            }
        }
    }

    //returns true when bullet was succesfully shot
    bool ShootBullet()
    {
        if(isBulletInChamber)
        {
            if(attachedMag == null || attachedMag.isMagEmpty())
            {
                isBulletInChamber = false;
            }
            attachedMag?.decrementBulletCout();
            shotTimer = 0;
            GameObject bullet = Instantiate(bulletTipPrefab, bulletSpawnPosition.position, bulletSpawnPosition.rotation);
            bullet.GetComponent<Rigidbody>().linearVelocity = bullet.transform.forward * bulletSpeed;
            Destroy(bullet, bulletTimeOut);
            Eject(shellPrefab);
            return true;
        }

        return false;
    }

    void Eject(GameObject o)
    {
        GameObject ejected = Instantiate(o, shellSpawnPosition.position, shellSpawnPosition.rotation);
        Rigidbody ejectedRB = ejected.GetComponent<Rigidbody>();
        ejectedRB.linearVelocity = ejected.transform.right * shellEjectSpeed;
        ejectedRB.angularVelocity = Random.insideUnitSphere * Random.Range(0, maxShellRotationSpeed);
        Destroy(ejected, shellTimeOut);
    }
}
