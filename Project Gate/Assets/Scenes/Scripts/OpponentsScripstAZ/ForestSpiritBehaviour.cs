using UnityEngine;

public class ForestSpiritBehaviour : OpponentBase
{
    private float healCooldown = 10f;
    private float healTimer = 0f;
    private bool isAggressive = false;
    private bool isAttacking = false;

    public float healAmount = 20f;
    public float healRange = 900f;
    public Animator animator;
    public LayerMask allyLayer; // warstwa na kt�rej s� potworki
    public float attackDelay = 0.5f;


    public float updateRate = 0.5f; // co ile sekund aktualizować cel
    public float followDistance = 4f;
    private float nextUpdateTime = 0f;

    public  void Start()
    {
        currentHealth = 10;
        maxHealth = currentHealth;
        defense = 10;
        baseAttack = 15;

        //agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

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
       
        FaceTarget();

        if (!isAggressive)
        {
            HealAllies();
            CheckIfAlone();
        }
        else
        {
            
            Attack();
        }
        Move();
    }
   
    public  void Move() 
    {
        if (player == null || isAttacking ) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
       // Debug.Log($"Distance: {distance}");
        if (currentHealth <= 0)
        {
            agent.isStopped = true;
            agent.ResetPath();
            return;
        }

        if (distance <= 4.0f)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            //Debug.Log("zatrzymaj sie");
            agent.isStopped = true;
            agent.ResetPath();
        }
        else 
        {
      
            if (Time.time >= nextUpdateTime)
            {
                nextUpdateTime = Time.time + updateRate;

                GetComponent<Rigidbody>().isKinematic = true;
                agent.isStopped = false;

                Vector3 direction = (transform.position - player.transform.position).normalized;
                Vector3 targetPosition = player.transform.position + direction * followDistance;
               // Debug.Log("idziemy");
                agent.SetDestination(player.transform.position);
            }


        }
    }

    public void Attack()
    {
        CheckIfAlone();
        if (isAttacking || player == null || Vector3.Distance(transform.position, player.transform.position) >= 1)
            return;
        //Debug.Log("is aggresive)");
        if (!isAggressive) return; // nie atakuje p�ki spokojny

        damageTimer += Time.fixedDeltaTime;

        if (damageTimer >= damageInterval)
        {
            isAttacking = true;
        agent.isStopped = true;

        animator.SetTrigger("Attack");

        Invoke(nameof(DoAttack), attackDelay);
            damageTimer = 0f;
        }

     void DoAttack()
    {

        Vector3 toPlayer = player.transform.position - transform.position;

        if ( Vector3.Distance(transform.position, player.transform.position) <= 3)
        {
                player.TakeDamage(40); // du�o wi�ksze obra�enia
               
            }
        }

        isAttacking = false;
        agent.isStopped = false;
    }


    private void HealAllies()
    {
        healTimer += Time.deltaTime;
        if (healTimer < healCooldown) return;

        healTimer = 0f;

        Collider[] allies = Physics.OverlapSphere(transform.position, healRange, allyLayer);

        // Szukaj kogo�, kto NIE jest sob� i nie jest martwy
        foreach (var allyCollider in allies)
        {
            OpponentBase ally = allyCollider.GetComponent<OpponentBase>();
            if (ally != null && ally != this && ally.currentHealth > 0 && ally.currentHealth < ally.maxHealth * 0.9f)
            {
                ally.currentHealth = Mathf.Min(ally.maxHealth, ally.currentHealth + healAmount);
                Debug.Log($"{name} healed {ally.name} for {healAmount} HP!");
                break; // lecz tylko jednego
            }
        }
    }

    private void CheckIfAlone()
    {
        Collider[] allies = Physics.OverlapSphere(transform.position, healRange, allyLayer);

        bool hasAllies = false;
        foreach (var allyCollider in allies)
        {
            OpponentBase ally = allyCollider.GetComponent<OpponentBase>();
            if (ally != null && ally != this && ally.currentHealth > 0)
            {
                hasAllies = true;
                isAggressive = false;
                break;
            }
        }

        if (!hasAllies)
        {
            isAggressive = true;
        }
    }

}
