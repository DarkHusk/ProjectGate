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

    public override void Start()
    {
        base.Start();

        if (modelTransform == null)
        {
            modelTransform = transform;
        }

        agent.enabled = true;
        agent.stoppingDistance = 1f;
        modelOffset = Vector3.up * flyHeight;
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
    }

    public override void Move()
    {
        if (player == null || isDiving) return;
        Vector3 targetPos = player.transform.position;
        targetPos.y = transform.position.y;
        agent.SetDestination(targetPos);
    }

    IEnumerator DiveAttack()
    {
        
        isDiving = true;
        agent.enabled = false;

        Vector3 diveTarget = player.transform.position;// + Vector3.up * 1f;

        // Pikowanie w dó³
        while (Vector3.Distance(modelTransform.position, diveTarget) > 0.5f)
        {
            modelTransform.position = Vector3.MoveTowards(
                modelTransform.position,
                diveTarget,
                Time.deltaTime * 5f * diveSpeedMultiplier
            );
            yield return null;
        }

        // Atak, jeœli gracz wci¹¿ w zasiêgu
        if (Vector3.Distance(modelTransform.position, player.transform.position) <= diveAttackRange)
        {
            player.TakeDamage(baseAttack * 2);
            
        }

        yield return new WaitForSeconds(0.5f);

        // Powrót na wysokoœæ
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