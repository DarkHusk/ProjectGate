using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vampire : OpponentBase
{
    public float attackDelay = 0.5f;
    public float attackAngle = 60f;
    public Animator animator;

    private bool isAttacking = false;
    private int noHealAttack;

    public void Start()
    {
        currentHealth = 300;
        maxHealth = currentHealth;
        defense = 10;
        baseAttack = 15;
        noHealAttack = 0;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

       // player = FindObjectOfType<PlayerTest>(); // ONLY IF THERE IS ONE PLAYER ; change it maybe?
        CreateHealthBar();

        speed = 1.5f;
        if (agent != null)
        {
            agent.speed = speed;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    void Update()
    {
        Move();
        FaceTarget();
        Attack();

    }

    public void Move()
    {
       
        if (player == null || isAttacking) return;

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

    public void Attack()
    {
        if (isAttacking || player == null || Vector3.Distance(transform.position, player.transform.position) >= 2)
            return;


        isAttacking = true;
        agent.isStopped = true;


        animator.SetTrigger("Attack");

        Invoke(nameof(DoAttack), attackDelay);
    }

    private void DoAttack()
    {
       
        Vector3 toPlayer = player.transform.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, toPlayer);

        if ( Vector3.Distance(transform.position, player.transform.position) <= 2)
        {
            player.TakeDamage(baseAttack);
            noHealAttack += 1;
            float doesAttackHeal = Random.Range(0f, 1f);
            if (doesAttackHeal < 0.1 && noHealAttack==3)
            { 
                float healing = Random.Range(0.2f, 0.4f) * baseAttack;
                if(currentHealth + healing <= maxHealth)
                {
                   // Debug.Log("vamp healed : " + currentHealth );
                    currentHealth += healing;
                }
                noHealAttack = 0;
            }
            else if( doesAttackHeal< 0.4 && noHealAttack == 3)
            {
                float healing = Random.Range(0.1f, 0.15f) * baseAttack;
                if (currentHealth + healing <= maxHealth)
                {
                    //Debug.Log("vamp healed : " + currentHealth);
                    currentHealth += healing;
                }
                noHealAttack = 0;
            }
            if( noHealAttack == 3) noHealAttack = 0;
           

        }

        isAttacking = false;
        agent.isStopped = false;
    }
}
