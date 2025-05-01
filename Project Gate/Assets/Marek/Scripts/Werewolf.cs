using UnityEngine;
using UnityEngine.AI;

public class Werewolf : OpponentBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        defense = 5;
        baseAttack = 5;
        currentHealth = 100;
        maxHealth = currentHealth;
        attackRange = 3;

        agent = GetComponent<NavMeshAgent>();

        player = FindObjectOfType<PlayerTest>();
        CreateHealthBar();
    }

    public override void Attack() 
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= 3)
        {

            player.TakeDamage(baseAttack);
            Debug.Log("atak");
        }

    }

}
