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
   /* public Vector3 targetPosition;
public float flightTime = 1.0f;

void Start()
{
    Destroy(gameObject, 8f);

    Rigidbody rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.useGravity = true;

        Vector3 displacement = targetPosition - transform.position;
        Vector3 horizontalDisplacement = new Vector3(displacement.x, 0, displacement.z);
        float horizontalDistance = horizontalDisplacement.magnitude;

        float verticalDisplacement = displacement.y;
        Vector3 horizontalDirection = horizontalDisplacement.normalized;

        float horizontalSpeed = horizontalDistance / flightTime;
        float verticalSpeed = (verticalDisplacement + 0.5f * Physics.gravity.y * flightTime * flightTime) / flightTime;

        Vector3 velocity = horizontalDirection * horizontalSpeed + Vector3.up * verticalSpeed;
        rb.velocity = velocity;
    }
}*/

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

