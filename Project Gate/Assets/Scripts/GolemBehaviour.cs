using UnityEngine;
using System.Text;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

public class Golem : OpponentBase
{
    public float attackDelay = 0.5f; 
    public float attackAngle = 60f;  
    public Animator animator;

    private bool isAttacking = false;

    public override void Start()
    {
        currentHealth = 300;
        maxHealth = currentHealth;
        defense = 10;
        baseAttack = 15;

        agent = GetComponent<NavMeshAgent>();

        player = FindObjectOfType<PlayerTest>(); // ONLY IF THERE IS ONE PLAYER ; change it maybe?
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

    public override void Move()
    {
        if (player == null || isAttacking) return;

        if (currentHealth > 0 && Vector3.Distance(transform.position, player.transform.position) >= 1)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    public override void Attack()
    {
        if (isAttacking || player == null || Vector3.Distance(transform.position, player.transform.position) >= 1)
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

        if (angleToPlayer <= attackAngle / 2f && Vector3.Distance(transform.position, player.transform.position) <= 2)
        {
            player.TakeDamage(20); 
        }

        isAttacking = false;
        agent.isStopped = false;
    }
}
