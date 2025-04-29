using UnityEngine;

public class Werewolf : OpponentBase
{
    public float attackDelay = 0.5f;
    public float attackAngle = 60f;
    public Animator animator;

    private bool isAttacking = false;

    [SerializeField] private float orbitRadius = 3f; // promie� okr��ania
    [SerializeField] private float orbitSpeed = 2f; // pr�dko�� poruszania si� wok� gracza
    [SerializeField] private float backstabAngleThreshold = 60f; // zakres k�towy za plecami gracza
    [SerializeField] private float attackDistance = 2f; // dystans do ataku

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

    }

    public  void Move()
    {
        if (player == null || isAttacking || currentHealth <= 0) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Zatrzymaj się, jeśli zbyt blisko gracza
       /* if (distanceToPlayer < orbitRadius * 0.9f)
        {
            agent.SetDestination(transform.position);
            return;
        }*/

        if (!isCircling)
        {
            if (distanceToPlayer > orbitRadius + 0.5f)
            {
                agent.SetDestination(player.transform.position); // Biegnij w stronę gracza
            }
            else
            {
                isCircling = true;
                orbitAngle = Random.Range(0f, 360f); // Losowy punkt startowy orbity
            }
        }
        else
        {
            // Płynne krążenie 

            orbitAngle += orbitSpeed * Time.deltaTime;

            // Oblicz pozycję na orbicie
            Vector3 offset = new Vector3(
                Mathf.Cos(orbitAngle * Mathf.Deg2Rad),
                0,
                Mathf.Sin(orbitAngle * Mathf.Deg2Rad)
            ) * orbitRadius;

            Vector3 orbitPosition = player.transform.position + offset;
            agent.SetDestination(orbitPosition);

           

            // Sprawdź, czy można zaatakować (backstab)
            Vector3 toEnemy = (transform.position - player.transform.position).normalized;
            float angle = Vector3.Angle(player.transform.forward, toEnemy);

            if (distanceToPlayer <= attackDistance && angle < backstabAngleThreshold)
            {
                isCircling = false;
                Attack();
                Debug.Log("were attacks " );
            }
           
        }
        Debug.DrawLine(transform.position, agent.destination, Color.red);
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
