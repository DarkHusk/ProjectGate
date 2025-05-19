using UnityEngine;
using System.Text;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

public class OpponentBase : MonoBehaviour
{

     
    protected int defense;
    protected int baseAttack;
    protected float damageInterval = 1f; // time in sec
    protected float damageTimer = 0f;
    protected float attackRange;
    protected int enemyValue;
    protected float _currentHealth;
    protected  GameObject healthBar;
    protected  Transform healthBarTransform;
    public  Vector3 healthBarOffset = new Vector3(0, 2f, 0); // Offset above the enemy


    public NavMeshAgent agent;
    public float maxHealth;
    public PlayerTest player;
    public float speed ;
    public ParticleSystem deathEffect;
     
    
    public float currentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
            UpdateHealthBar();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start() //virtual
    {
        currentHealth = 100;
        maxHealth = currentHealth;
        defense = 10;
        baseAttack = 5;

        agent = GetComponent<NavMeshAgent>();

        player = FindObjectOfType<PlayerTest>(); // ONLY IF THERE IS ONE PLAYER ; change it maybe?
        CreateHealthBar();

    }

    public virtual void Update()
    {
        Move();
        FaceTarget();
       

    }


   public virtual void Move() // virtual, bo to base ; change how it works
    {
        if (player == null) return;
        
       
        if (currentHealth > 0 && Vector3.Distance(transform.position, player.transform.position) >= 1)
        {
            agent.SetDestination(player.transform.position);
            
        }
    }

    public void Die() 
    {
        
        if (healthBar != null)
        {
            Destroy(healthBar);
        }

        if (deathEffect != null)
        {
            deathEffect.Play(); // displays the effect
        }

        Destroy(gameObject, 2f);   // after 2 seconds, destroy object
    }


   

    public virtual void Attack() // virtual
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= 3)
        {
            damageTimer += Time.fixedDeltaTime;

            if (damageTimer >= damageInterval)
            {
                player.TakeDamage(baseAttack);
                damageTimer = 0f;
            }
        }

    }
    public void TakeDamage(float damage)
    {

        if (currentHealth > 0)
        {
            currentHealth -= damage;
           
        }
        if(currentHealth <= 0) Die();
    }


    protected void CreateHealthBar()
    {
        // Create a health bar
        healthBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
    

        healthBarTransform = healthBar.transform;
        Destroy(healthBar.GetComponent<Collider>()); // Remove the collider

        // Set parent and position
        healthBarTransform.SetParent(transform);
        healthBarTransform.localPosition = healthBarOffset;
        healthBarTransform.localRotation = Quaternion.Euler(90, 2, 2);

        var healthBarRenderer = healthBar.GetComponent<Renderer>();
        //healthBarRenderer.material = new Material(Shader.Find("Standard"));

        UpdateHealthBar();
        //Debug.LogError("atakujowany przeciwnik");
    }

    protected void UpdateHealthBar()
    {
        if (healthBar == null)
        {
            return;
        }

        float healthPercentage = _currentHealth / maxHealth;
        healthBarTransform.localScale = new Vector3(healthPercentage * 4, 0.1f, 0.1f);

        // Change color based on health percentage
        var healthBarRenderer = healthBar.GetComponent<Renderer>();
        healthBarRenderer.material.color = Color.Lerp(Color.red, Color.green, healthPercentage);

        // Hide health bar when dead
        if (_currentHealth<=0)
        {
            healthBar.SetActive(false);
        }
    }

    protected void FaceTarget(){
        
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

}
