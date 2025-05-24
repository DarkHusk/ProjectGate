using UnityEngine;

public class Werewolf : OpponentBase
{
    public float attackDelay = 0.5f;
    public float attackAngle = 60f;
    public Animator animator;

    private bool isAttacking = false;

    [SerializeField] private float orbitRadius = 2f; // promie� okr��ania
    [SerializeField] private float orbitSpeed = 40f; // pr�dko�� poruszania si� wok� gracza
    [SerializeField] private float backstabAngleThreshold = 60f; // zakres k�towy za plecami gracza
    [SerializeField] private float attackDistance = 1f; // dystans do ataku

    private bool isCircling = false;
    private float orbitAngle = 0f;


    public override void Start()
    {
        currentHealth = 300;
        maxHealth = currentHealth;
        defense = 10;
        baseAttack = 15;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

       // player = FindObjectOfType<PlayerTest>(); // ONLY IF THERE IS ONE PLAYER ; change it maybe?
        CreateHealthBar();

        speed = 4f;
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

    }

    public void Move()
    {
        if (player == null || isAttacking || currentHealth <= 0) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (!isCircling)
        {
            if (distanceToPlayer > orbitRadius + 0.5f)
            {
                agent.SetDestination(player.transform.position);
            }
            else
            {
                isCircling = true;

                
                Vector3 direction = (transform.position - player.transform.position).normalized;
                orbitAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            }
        }
        else
        {
            
            orbitAngle += orbitSpeed * Time.deltaTime;
            float dynamicRadius = orbitRadius + Mathf.Sin(Time.time * 0.5f) * 0.5f;
            Vector3 offset = new Vector3(
                Mathf.Cos(orbitAngle * Mathf.Deg2Rad),
                0,
                Mathf.Sin(orbitAngle * Mathf.Deg2Rad)
            ) * dynamicRadius;

            Vector3 orbitTarget = player.transform.position + offset;

            agent.SetDestination(orbitTarget);
            CheckAttackCondition(distanceToPlayer);

            Debug.DrawLine(transform.position, orbitTarget, Color.red);
        }
    }

    private void CheckAttackCondition(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackDistance)
        {
            Vector3 toEnemy = (transform.position - player.transform.position).normalized;
            float angle = Vector3.Angle(player.transform.forward, toEnemy);

           if (angle > 180f - backstabAngleThreshold / 2f && angle < 180f + backstabAngleThreshold / 2f)
            {
                isCircling = false;
                Attack();
                Debug.Log("Atak od tyłu!");
            }
        }
    }




    public void Attack()
    {
        if (!isCircling)
        {
            if (isAttacking || player == null || Vector3.Distance(transform.position, player.transform.position) >= 1)
            {
                return;
            }

            isAttacking = true;
            agent.isStopped = true;


            animator.SetTrigger("Attack");

            Invoke(nameof(DoAttack), attackDelay);
        }
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
