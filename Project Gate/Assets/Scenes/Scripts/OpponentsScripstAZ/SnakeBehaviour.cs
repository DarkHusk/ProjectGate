using UnityEngine;
using  System.Collections;

public class Snake : OpponentBase
{

    private Coroutine poisonCoroutine; 
    public bool isPoisoned = false;

    public float attackDelay = 0.5f;
    public float attackAngle = 60f;
    public Animator animator;

    protected bool isAttacking = false;

    public void Start()
    {
        currentHealth = 300;
        maxHealth = currentHealth;
        defense = 10;
        baseAttack = 15;

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

    public  void Move()
    {
        if (player == null || isAttacking) return;

        if (currentHealth > 0 && Vector3.Distance(transform.position, player.transform.position) >= 1)
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

        if ( Vector3.Distance(transform.position, player.transform.position) <= 2)
        {
            player.TakeDamage(20);
           // Debug.Log("bijemy nie kor");
            PoisonPlayer();
        }

        isAttacking = false;
        agent.isStopped = false;
    }

    public void PoisonPlayer()
    {
        if (isPoisoned)
        {
            StopCoroutine(poisonCoroutine);
        }

        poisonCoroutine = StartCoroutine(PoisonCoroutine());
    }

    private IEnumerator PoisonCoroutine()
    {
        isPoisoned = true;

        float poisonDuration = 5f;
        float tickInterval = 1f;
        float damagePerTick = 5f;

        float elapsed = 0f;

        while (elapsed < poisonDuration)
        {
            player.TakeDamage(damagePerTick);
            //Debug.Log("bijemy");
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        isPoisoned = false;
    }

}
