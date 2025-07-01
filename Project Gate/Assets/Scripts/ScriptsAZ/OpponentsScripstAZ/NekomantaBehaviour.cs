using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nekomancer : OpponentBase
{
    public int maxSummons = 3;
    public float summonCooldown = 5f;
    public float summonRadius = 8f;

    public Animator animator;

    public LayerMask collisionLayer;
    public LayerMask playerCollisionLayer;

    public GameObject minionPrefab;

    private float summonTimer = 0f;
    private List<GameObject> summonedMinions = new List<GameObject>();



    public float updateRate = 0.5f; // co ile sekund aktualizowaæ cel
    public float followDistance = 4f;
    private float nextUpdateTime = 0f;



    public override void Start()
    {
        spawnTime = Time.time;
        base.Start();
        enemyValue = 5;
        defense = 3;
        baseAttack = 0; // doesn't attack directly
        currentHealth = 80;
        maxHealth = currentHealth;
        player = FindObjectOfType<PlayerTest>();
        CreateHealthBar();

        speed = 5f;
        if (agent != null)
        {
            agent.speed = speed;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }


        if (agent == null)
        {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }
        StartCoroutine(SnapToNavMesh());
    }

    public override void Update()
    {
        base.Update();

        CleanUpDestroyedMinions();

        summonTimer += Time.deltaTime;
        if (summonedMinions.Count < maxSummons && summonTimer >= summonCooldown)
        {
            SummonMinion();
            summonTimer = 0f;
        }
        if (Time.deltaTime % summonCooldown == 0)
        {
            animator.ResetTrigger("Summon");
            if (Time.time >= nextUpdateTime && agent.isActiveAndEnabled && Vector3.Distance(transform.position, player.transform.position) >= followDistance)
                animator.ResetTrigger("Stopped");
        }

        StartCoroutine(SnapToNavMesh());
        if (!agent.isOnNavMesh && Time.time - spawnTime >= 5f)
        {
            _currentHealth = 0;
        }
    }

    public void Move()
    {
        if (player == null || currentHealth <= 0)
        {
            if (agent.isActiveAndEnabled)
            {
                animator.SetTrigger("Stopped");
                agent.isStopped = true;
                agent.ResetPath();
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);

        // Zatrzymaj siê jeœli jesteœ wystarczaj¹co blisko
        if (distance <= followDistance)
        {
            if (agent.isActiveAndEnabled)
            {
                animator.SetTrigger("Stopped");
                agent.isStopped = true;
                agent.ResetPath();
            }
        }
        else
        {
            // Poruszaj siê tylko jeœli czas na aktualizacjê
            if (Time.time >= nextUpdateTime && agent.isActiveAndEnabled)
            {
                nextUpdateTime = Time.time + updateRate;
                agent.isStopped = false;
                
                agent.SetDestination(player.transform.position);
            }
        }
    }
    /*  public override void Attack()
      {
          // Nekomancer nie atakuje sam
      }*/

    void SummonMinion()
    {
        animator.SetTrigger("Stopped");
        Vector3 spawnPosition = GetRandomValidPosition();
        if (spawnPosition != Vector3.zero)
        {
            animator.SetTrigger("Summon");
            GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
            summonedMinions.Add(minion);
            Debug.Log("Nekomancer summoned minion. Total: " + summonedMinions.Count);
        }
        else
        {
            Debug.LogWarning("Nekomancer couldn't find a valid position to summon.");
        }
        
    }

    Vector3 GetRandomValidPosition()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * summonRadius;
            Vector3 tryPos = new Vector3(randomCircle.x, 0, randomCircle.y) + transform.position;

            int combinedLayer = collisionLayer | playerCollisionLayer;
            Collider[] colliders = Physics.OverlapSphere(tryPos, 1f, combinedLayer);
            if (colliders.Length == 0)
                return tryPos;
        }
        return Vector3.zero; // failed to find position
    }

    void CleanUpDestroyedMinions()
    {
        summonedMinions.RemoveAll(minion => minion == null);
    }

    public void Destroy()
    {
        foreach (GameObject minion in summonedMinions)
        {
            if (minion != null)
                Destroy(minion);
        }
    }
}

