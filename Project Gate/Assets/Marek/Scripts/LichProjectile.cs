using UnityEngine;

public class LichProjectile : MonoBehaviour
{
    public float speed = 10f;
    public OpponentBase attacker; // Or whatever class has baseDamage

    void Start()
    {
        Destroy(gameObject, 8f);
        GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
    }
   

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Projectile hit: " + other.name);
        var target = other.GetComponentInParent<PlayerTest>();
        if (target != null && attacker != null)
        {
            target.TakeDamage(attacker.baseAttack);
        }

        Destroy(gameObject);
    }

}

