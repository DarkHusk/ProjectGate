using UnityEngine;
using System.Collections;

public class FlyingDemon : OpponentBase
{
    public float flyHeight = 5f;
    public float diveSpeedMultiplier = 3f;
    public float diveCooldown = 3f;
    public float diveAttackRange = 2f;
    public float hoverSmoothness = 5f;
    public float attackTriggerDistance = 2f; 
    public Transform modelTransform;

    bool isDiving = false;
    float diveTimer = 0f;
    Vector3 modelOffset = Vector3.zero;

    public Animator animator;

    public override void Start()
    {
        spawnTime = Time.time;
        base.Start();
        player = FindObjectOfType<PlayerTest>();
        if (modelTransform == null)
        {
            modelTransform = transform;
        }
        enemyValue = 4;

        agent.enabled = true;
        agent.stoppingDistance = 1f;
        modelOffset = Vector3.up * flyHeight;
        speed = 5f;
        if (agent != null)
        {
            agent.speed = speed;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        StartCoroutine(SnapToNavMesh());
    }

    public override void Update()
    {
        base.Update();

        if (player == null) return;

        diveTimer += Time.deltaTime;
        UpdateModelPosition();

        //if (!isDiving)
        {
            Move();


            if (Vector3.Distance(transform.position, player.transform.position) <= attackTriggerDistance
                && diveTimer >= diveCooldown && !isDiving)
            {
                StartCoroutine(DiveAttack());
                diveTimer = 0f;
            }
        }
        if (!agent.isOnNavMesh && Time.time - spawnTime >= 5f)
        {
            _currentHealth = 0;
        }

    }
    public void Move()
    {

        if (player == null || isDiving) return;

        if (currentHealth > 0 && Vector3.Distance(transform.position, player.transform.position) >= 1.5)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

   /* public override void Move()
    {
        if (player == null || isDiving) return;
        Vector3 targetPos = player.transform.position;
        targetPos.y = transform.position.y;
        agent.SetDestination(targetPos);
    }*/

    IEnumerator DiveAttack()
    {
        animator.SetTrigger("Attack");

        isDiving = true;
        agent.enabled = false;

        Vector3 diveTarget = player.transform.position;// + Vector3.up * 1f;

        // Pikowanie w d�
        while (Vector3.Distance(modelTransform.position, diveTarget) > 0.5f)
        {
            modelTransform.position = Vector3.MoveTowards(
                modelTransform.position,
                diveTarget,
                Time.deltaTime * 5f * diveSpeedMultiplier
            );
            yield return null;
        }

        // Atak, je�li gracz wci�� w zasi�gu
        if (Vector3.Distance(modelTransform.position, player.transform.position) <= diveAttackRange)
        {
            player.TakeDamage(baseAttack * 2);
            
        }

        yield return new WaitForSeconds(0.5f);

        // Powr�t na wysoko��
        Vector3 returnPos = transform.position + modelOffset;
        while (Vector3.Distance(modelTransform.position, returnPos) > 0.5f)
        {
            modelTransform.position = Vector3.MoveTowards(
                modelTransform.position,
                returnPos,
                Time.deltaTime * 5f
            );
            yield return null;
        }

        isDiving = false;
        agent.enabled = true;
        animator.ResetTrigger("Attack");
    }

    void UpdateModelPosition()
    {
        if (!isDiving && modelTransform != null)
        {
            Vector3 targetPos = transform.position + modelOffset;
            modelTransform.position = Vector3.Lerp(
                modelTransform.position,
                targetPos,
                Time.deltaTime * hoverSmoothness
            );
            FaceTarget();
        }
    }
}