//Marek

using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

public class ShadowPanthershark : OpponentBase
{
    public LayerMask groundLayer;
    public GameObject gfxObject;
    public float hideThreshold = 1f;
    public float emergeThreshold = 1f;
    private BehaviorGraphAgent behAgent;
    private Collider objectCollider;
    private GameObject groundProjection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        defense = 5;
        baseAttack = 20;
        currentHealth = 300;
        maxHealth = currentHealth;
        attackRange = 3;
        enemyValue= 3;

        objectCollider = GetComponent<Collider>();

        if (gfxObject == null)
        {
            gfxObject = transform.GetChild(0).gameObject; // Assuming GFX is the first child
        }
        groundProjection = new GameObject("GroundProjection");
        groundProjection.SetActive(false);

        agent = GetComponent<NavMeshAgent>();
        behAgent = GetComponent<BehaviorGraphAgent>();
        player = FindObjectOfType<PlayerTest>();
        if (behAgent != null && player != null)
        {
            behAgent.SetVariableValue("Target", player.gameObject);
        }
        else
        {
            Debug.LogWarning("Could not assign Target: missing agent or player.");
        }
        CreateHealthBar();
    }

    public void Attack(int whichAttack)
        /*
         * 0 - neutral state if ever needed
         * 1 - Wide attack
         * 2 - Standing attack
         * 3 - Bite
         * 4 - Chaaaaarge
         */
    {
        int tempAttack = 1;
        float tempAttackRange =1f;
        float tempAttackAngle =1f;

        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        if (whichAttack == 1)
        {
            //Debug.Log("SPanthS attack 1");
            //Wide
            tempAttack = baseAttack;
            tempAttackRange = attackRange;
            tempAttackAngle = 60f;
        }
        else if (whichAttack == 2)
        {
            //Debug.Log("SPanthS attack 2");
            //Standing
            tempAttack = (int)((float)baseAttack * 1.5f);
            tempAttackRange = attackRange + 2;
            tempAttackAngle = 30f;
        }
        else if (whichAttack == 3)
        {
            //Bite
            //Debug.Log("SPanthS attack 3");
            tempAttack = (int)((float)baseAttack * 1.25f);
            tempAttackRange = attackRange;
            tempAttackAngle = 30f;
        }
        else if (whichAttack == 4)
        {
            //Chaaarge
        }
        if (distanceToPlayer <= tempAttackRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= tempAttackAngle)
            {
                player.TakeDamage(tempAttack);
            }
        }
        if (whichAttack == 2)//second paw dmg
        {
            float timer = 0.5f;
            while(timer > 0) { timer -= Time.deltaTime; } //time between first and second paw dmg
            if (distanceToPlayer <= tempAttackRange)
            {
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleToPlayer <= tempAttackAngle)
                {
                    player.TakeDamage(tempAttack);
                }
            }
        }
    }


    public void HideObjectUnderground()
    {
        // Cast a ray downward to find the surface of the terrain (or ground)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            float undergroundHeight = hit.point.y - hideThreshold;  
            gfxObject.transform.position = new Vector3(transform.position.x, undergroundHeight, transform.position.z);
            gfxObject.SetActive(false);
            objectCollider.isTrigger = true;
            CreateGroundProjection(hit.point);
        }
    }
    private void CreateGroundProjection(Vector3 groundPosition)
    {
        groundProjection.transform.SetParent(this.transform);

        // Offset slightly to avoid z-fighting
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            groundProjection.transform.position = hit.point + hit.normal * 0.05f;

            // First align "up" with the normal
            Quaternion alignToSurface = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // to rotate if not flat (resolved)
            Quaternion rotateFlat = Quaternion.Euler(0, 0, 0);

            groundProjection.transform.rotation = alignToSurface * rotateFlat;
        }

        groundProjection.transform.localScale = Vector3.one * 2f;
        groundProjection.SetActive(true);

        MeshRenderer mr = groundProjection.GetComponent<MeshRenderer>();
        if (mr == null)
        {
            MeshFilter mf = groundProjection.AddComponent<MeshFilter>();
            mr = groundProjection.AddComponent<MeshRenderer>();

            // Create a quad mesh
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3(0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, 0.5f)
            };
            mesh.uv = new Vector2[]
            {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
            };
            mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
            mesh.RecalculateNormals();
            mf.mesh = mesh;

            // Generate procedural purple circle texture
            Texture2D tex = new Texture2D(256, 256, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Bilinear;
            Color clear = new Color(0, 0, 0, 0);
            Color purple = new Color(0.5f, 0f, 0.5f, 1f);
            float radius = tex.width / 2f;
            float rSqr = radius * radius;
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    float dx = x - radius;
                    float dy = y - radius;
                    float distSqr = dx * dx + dy * dy;
                    if (distSqr <= rSqr)
                    {
                        float alpha = 1f - (distSqr / rSqr); // Soft fade
                        tex.SetPixel(x, y, new Color(purple.r, purple.g, purple.b, alpha));
                    }
                    else
                    {
                        tex.SetPixel(x, y, clear);
                    }
                }
            }
            tex.Apply();

            Material mat = new Material(Shader.Find("Unlit/Transparent"));
            mat.mainTexture = tex;
            mr.material = mat;
        }
    }

    public void RestoreObject()
    {
        // Cast a ray downward to find the surface of the terrain (or ground)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 restoredPosition = new Vector3(transform.position.x, hit.point.y + hideThreshold, transform.position.z);

            // Before moving, check for any other objects in the path (to avoid overlap)
            /*if (IsPositionOccupied(restoredPosition))
            {
                // If occupied, adjust the position slightly to avoid overlap
                restoredPosition += new Vector3(0, emergeThreshold, 0); // Move slightly up
            }*/

            // Restore the GFX object to the correct position over the ground
            gfxObject.SetActive(true);
            //gfxObject.transform.position = restoredPosition;
            gfxObject.transform.position = transform.position;

            // Re-enable the collider (disable trigger mode)
            objectCollider.isTrigger = false;

            // Ensure the NavMeshAgent is updated (reposition the agent)
            if (agent != null)
            {
                agent.Warp(transform.position); // Ensure NavMeshAgent is correctly positioned
            }
        }

        // Deactivate the ground projection effect
        groundProjection.SetActive(false);
    }

    private bool IsPositionOccupied(Vector3 position)
    {
        int combinedLayer = LayerMask.GetMask("Monster", "WhatIsPlayer"); 
        Collider[] colliders = Physics.OverlapSphere(position, 1f, combinedLayer); 
        return colliders.Length > 0; // If any colliders exist, return true (position is occupied)
    }



    private void OnDrawGizmosSelected()
    {
        float attackAngle = 90f;
        float gizmoHeight = 2f; //height included

        Vector3 origin = transform.position + Vector3.up * gizmoHeight;

        Gizmos.color = Color.yellow;

        Vector3 forward = transform.forward;
        Vector3 leftLimit = Quaternion.Euler(0, -attackAngle / 2, 0) * forward;
        Vector3 rightLimit = Quaternion.Euler(0, attackAngle / 2, 0) * forward;

        Gizmos.DrawRay(origin, forward * attackRange);
        Gizmos.DrawRay(origin, leftLimit * attackRange);
        Gizmos.DrawRay(origin, rightLimit * attackRange);
    }

}