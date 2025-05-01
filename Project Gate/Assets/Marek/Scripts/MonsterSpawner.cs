using UnityEngine;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public float spawnRadius = 10f;
    public int initialnumberOfMonsters = 5;
    public int maxNumberOfMonsters = 10;
    public LayerMask collisionLayer; //for monster collision detection
    public LayerMask playerCollisionLayer; //for player collision detection
    public Material transparentRed;
    public Material transparentGreen;

    public float cooldown = 15;
    public float minCooldown = 10;
    public float maxCooldown = 20;
    private float spawnTimer = 0f;

    public bool portalDefending = false, isPlayerInRange;
    private float defenseProgress = 0f;
    private float oneSecTimer = 0f;
    public float defendRadius = 20f;
    public float defenseActivationRadius = 10f;
    private GameObject defendRadiusSphere;
    private GameObject defenseActivationRadiusSphere;
    public bool teleportationTrigger = false;
    private float afterTimer = 15f;

    private List<GameObject> spawnedMonsters = new List<GameObject>();

    void Start()
    {
        InitialSpawnMonsters();
        if (defenseActivationRadiusSphere == null)
        {
            SpawnTransparentGreenSphere(transform.position, defenseActivationRadius);
        }
    }

    void InitialSpawnMonsters()
    {
        int monstersSpawned = 0;
        while (monstersSpawned < initialnumberOfMonsters)
        {
            Vector3 spawnPosition = GetRandomPositionWithinCircle();
            if (IsPositionValid(spawnPosition))
            {
                GameObject newMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
                spawnedMonsters.Add(newMonster);
                monstersSpawned++;
            }
        }
    }
    void SpawnMonsters()
    {
        CleanUpDestroyedMonsters();
        if (spawnedMonsters.Count < maxNumberOfMonsters)
        {
            Vector3 spawnPosition = GetRandomPositionWithinCircle();
            if (IsPositionValid(spawnPosition))
            {
                GameObject newMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
                spawnedMonsters.Add(newMonster);
            }
        }
    }

    Vector3 GetRandomPositionWithinCircle()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(randomCircle.x, 0, randomCircle.y) + transform.position;
    }

    bool IsPositionValid(Vector3 position)
    {
        int combinedLayer = collisionLayer | playerCollisionLayer;
        Collider[] colliders = Physics.OverlapSphere(position, 1f, combinedLayer);
        return colliders.Length == 0;
    }

    public void CleanUpDestroyedMonsters()
    {
        spawnedMonsters.RemoveAll(monster => monster == null);
    }

    public void SpawnTransparentRedSphere(Vector3 position, float radius)
    {
        if (transparentRed == null)
        {
            Debug.LogError("Nie znaleziono materia³u 'transparentRed.mat'");
            return;
        }

        defendRadiusSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        defendRadiusSphere.transform.position = position;
        defendRadiusSphere.transform.localScale = Vector3.one * radius * 2f;
        defendRadiusSphere.GetComponent<Renderer>().sharedMaterial = transparentRed;
    }


    public void SpawnTransparentGreenSphere(Vector3 position, float radius)
    {
        if (transparentGreen == null)
        {
            Debug.LogError("Nie znaleziono materia³u 'transparentGreen.mat'");
            return;
        }

        defenseActivationRadiusSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        defenseActivationRadiusSphere.transform.position = position;
        defenseActivationRadiusSphere.transform.localScale = Vector3.one * radius * 2f;
        defenseActivationRadiusSphere.GetComponent<Renderer>().sharedMaterial = transparentGreen;
    }
    private void Update()
    {
        if (defenseProgress < 100)//active until defense sequence is finished
        {
            spawnTimer += Time.deltaTime;

            //Portal defending phase check
            if (portalDefending == false)
            {
                isPlayerInRange = Physics.CheckSphere(transform.position, defenseActivationRadius, playerCollisionLayer);

                //defending sequence start
                if (isPlayerInRange == true)
                {
                    Debug.Log("DEFENSE START");
                    portalDefending = true;
                    minCooldown *= (2 / 3);
                    maxCooldown *= (2 / 3);
                    if (defenseActivationRadiusSphere != null)
                    {
                        Destroy(defenseActivationRadiusSphere);
                        defenseActivationRadiusSphere = null;
                    }
                    SpawnTransparentRedSphere(transform.position, defendRadius);
                }
            }
            else if (portalDefending == true)
            {
                oneSecTimer += Time.deltaTime;
                isPlayerInRange = Physics.CheckSphere(transform.position, defendRadius, playerCollisionLayer);
                if (isPlayerInRange == true && oneSecTimer >= 1f)
                {
                    defenseProgress += 1f;
                    oneSecTimer -= 1f;
                    Debug.Log("PROGRESS = " + defenseProgress);
                }
                else if (isPlayerInRange == false && oneSecTimer >= 1f)
                {
                    if (defenseProgress > 0f) { defenseProgress -= 0.25f; } else { defenseProgress = 0f; }//align to 0 if value too low
                    oneSecTimer -= 1f;
                    Debug.Log("PROGRESS = " + defenseProgress);
                }
            }


            //Spawning Over Time
            if (spawnTimer >= cooldown)
            {
                SpawnMonsters();
                spawnTimer = 0f;
                cooldown = Random.Range(minCooldown, maxCooldown);
            }
        }
        else//after defense
        {
            if (defendRadiusSphere != null)
            {
                Destroy(defendRadiusSphere);
                defendRadiusSphere = null;
            }
            afterTimer -= Time.deltaTime;
            if (afterTimer <= 0f)
            {
                teleportationTrigger= true;
                Debug.Log("TELEPORT");
            }
        }
    }
   
}