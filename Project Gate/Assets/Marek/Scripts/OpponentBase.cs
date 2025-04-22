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
    float damageInterval = 1f; // time in sec
    float damageTimer = 0f;
    protected float attackRange;
    int enemyValue;
    float _currentHealth;
    private GameObject healthBar;
    private Transform healthBarTransform;
    private Vector3 healthBarOffset = new Vector3(0, 2f, 0); // Offset above the enemy
    protected NavMeshAgent agent;


    public float maxHealth;
    public PlayerTest player;
    //public Transform player;
    public float speed;
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


    void Update()
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start() //virtual
    {
        currentHealth = 100;
        maxHealth = currentHealth;
        defense = 10;
        baseAttack = 5;

        agent = GetComponent<NavMeshAgent>();

        player = FindObjectOfType<PlayerTest>();
        CreateHealthBar();

    }

    public virtual void Attack() // virtual
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= 3)
        {
            damageTimer += Time.fixedDeltaTime;

            //if (damageTimer >= damageInterval)
            //{
                player.TakeDamage(baseAttack);
                damageTimer = 0f;
                Debug.Log("atak");
            //}
        }

    }
    public void TakeDamage(float damage)
    {

        if (currentHealth > 0)
        {
            currentHealth -= damage;

        }
        if (currentHealth <= 0) Die();
    }


    protected void CreateHealthBar()
    {
        // Create a health bar
        healthBar = GameObject.CreatePrimitive(PrimitiveType.Quad);
        healthBarTransform = healthBar.transform;
        Destroy(healthBar.GetComponent<Collider>()); // Remove the collider

        // Set parent and position
        healthBarTransform.SetParent(transform);
        healthBarTransform.localPosition = healthBarOffset;
        healthBarTransform.localRotation = Quaternion.Euler(90, 0, 0);

        var healthBarRenderer = healthBar.GetComponent<Renderer>();
        //healthBarRenderer.material = new Material(Shader.Find("Standard"));

        UpdateHealthBar();
        Debug.Log("healthBarcreated");
        //Debug.LogError("atakujowany przeciwnik");
    }

    void UpdateHealthBar()
    {
        if (healthBar == null)
        {
            return;
        }

        float healthPercentage = _currentHealth / maxHealth;
        healthBarTransform.localScale = new Vector3(healthPercentage * 4, 1f, 1);

        // Change color based on health percentage
        var healthBarRenderer = healthBar.GetComponent<Renderer>();
        healthBarRenderer.material.color = Color.Lerp(Color.red, Color.green, healthPercentage);

        // Hide health bar when dead
        if (_currentHealth <= 0)
        {
            healthBar.SetActive(false);
        }
    }

    void FaceTarget()
    {

        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

}
