using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

public class Lich : OpponentBase
{
    public int onStartSpawns = 1; //- 1 bo pierwszy Spawn()
    public int maxSpawns = 5;
    public float spawnRadius = 10f;
    public LayerMask collisionLayer; //for monster collision detection
    public LayerMask playerCollisionLayer; //for player collision detection
    public GameObject minionPrefab;
    public GameObject projectilePrefab;
    private BehaviorGraphAgent behAgent;
    private List<GameObject> spawnedMonsters = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        defense = 5;
        baseAttack = 100;
        currentHealth = 100;
        maxHealth = currentHealth;
        attackRange = 3;

        agent = GetComponent<NavMeshAgent>();
        behAgent= GetComponent<BehaviorGraphAgent>();

        player = FindObjectOfType<PlayerTest>();
        if (behAgent != null && player != null)
        {
            behAgent.SetVariableValue("Target", player.gameObject);
        }
        else
        {
            Debug.LogWarning("Could not assign Target: missing agent or player.");
        }
        for (int i = 0;i< onStartSpawns;i++)
        {
            Spawn();
        }

        CreateHealthBar();
    }

    /* public override void Attack() 
     {
         player.TakeDamage(baseAttack);
             Debug.Log("atak licz");
     }*/
    public override void Attack()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;

        Vector3 spawnPosition = transform.position + transform.forward * 1.5f + Vector3.up * 1.5f;

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
        
        

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * 32f, ForceMode.Impulse);
        }

        LichProjectile proj = projectile.GetComponent<LichProjectile>();
        if (proj != null)
        {
            proj.attacker = this;

            Collider lichCollider = GetComponent<Collider>();
            Collider projectileCollider = projectile.GetComponent<Collider>();
            if (lichCollider != null && projectileCollider != null)
            {
                Physics.IgnoreCollision(lichCollider, projectileCollider);
            }
            int layerMask = collisionLayer.value;

            Collider[] collidersToIgnore = FindObjectsOfType<Collider>();
            foreach (Collider col in collidersToIgnore)
            {
                if (((1 << col.gameObject.layer) & layerMask) != 0)
                {
                    Physics.IgnoreCollision(projectileCollider, col);
                }
            }
        }
        

        Debug.Log("Lich strzela w gracza");
    }

    public void Spawn()
    {
        if (spawnedMonsters.Count < maxSpawns)
        {
            Vector3 spawnPosition = GetRandomPositionWithinCircle();
            int loopCnt = 0;
            while (!IsPositionValid(spawnPosition) && loopCnt < 30)
            {
                spawnPosition = GetRandomPositionWithinCircle();
                loopCnt++;
            }
            if (loopCnt < 30)
            {
                GameObject newMonster = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
                spawnedMonsters.Add(newMonster);
                Debug.Log("Spawned " + spawnedMonsters.Count + " minions");
            }
            else
            {
                Debug.LogWarning("Failed to find a valid spawn position after 30 attempts.");
            }

        }
        else Debug.Log("Spawn maximmum reached");
    }

    Vector3 GetRandomPositionWithinCircle()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(randomCircle.x, 0, randomCircle.y) + transform.position;
    }

    bool IsPositionValid(Vector3 position)
    {
        int combinedLayer = collisionLayer | playerCollisionLayer;
        Collider[] colliders = Physics.OverlapSphere(position, 1f, combinedLayer);
        return colliders.Length == 0;
    }

    public void CleanUpDestroyedMonsters()
    {
        spawnedMonsters.RemoveAll(monster => monster == null);
    }

}
