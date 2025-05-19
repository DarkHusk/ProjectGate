using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

public class Minion : OpponentBase
{
    private BehaviorGraphAgent behAgent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        defense = 5;
        baseAttack = 5;
        currentHealth = 100;
        maxHealth = currentHealth;
        attackRange = 3;


        agent = GetComponent<NavMeshAgent>();
        behAgent = GetComponent<BehaviorGraphAgent>();
        player = FindObjectOfType<PlayerTest>();
        if (behAgent != null && player != null)
        {
            behAgent.SetVariableValue("Target", player.gameObject);
        }
        else
        {
            Debug.LogWarning("Could not assign Target: missing agent or player.");
        }
        CreateHealthBar();
    }

    public override void Attack()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= 5f)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            //if (angleToPlayer <= 45f) //90 degrees total
            //{
                player.TakeDamage(baseAttack);
                Debug.Log("atak w zasiegu i kacie");
            //}
        }
    }

    private void OnDrawGizmosSelected()
    {
        float attackRange = 3f;
        float attackAngle = 90f;
        float gizmoHeight = 2f; //height included

        Vector3 origin = transform.position + Vector3.up * gizmoHeight;

        Gizmos.color = Color.yellow;

        Vector3 forward = transform.forward;
        Vector3 leftLimit = Quaternion.Euler(0, -attackAngle / 2, 0) * forward;
        Vector3 rightLimit = Quaternion.Euler(0, attackAngle / 2, 0) * forward;

        Gizmos.DrawRay(origin, forward * attackRange);
        Gizmos.DrawRay(origin, leftLimit * attackRange);
        Gizmos.DrawRay(origin, rightLimit * attackRange);
    }

}
