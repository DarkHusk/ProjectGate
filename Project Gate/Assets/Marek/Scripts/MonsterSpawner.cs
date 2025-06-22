//Marek

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MonsterSpawner : MonoBehaviour
{
    public int enemyPointLimit = 30;
    public int currentEnemyPoints = 0; //can be private, public to be easily seen while debugging
    private const int spawnRollMinStart = 0;
    private const int spawnRollMaxStart = 100;
    private const int thresholdRandValue = 80;
    private const int thresholdenemyValue = 5;
    private const int spawningStep = 5;
    private int spawnRollMin = spawnRollMinStart;
    private int spawnRollMax = spawnRollMaxStart;

    public List<GameObject> monsterPrefabs = new List<GameObject>();
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
    public float defenseMaxTime = 100;
    public float defenseUpValue = 1;
    public float defenseDownValue = 0.25f;
    public float defendRadius = 20f;
    public float defenseActivationRadius = 10f;
    private GameObject defendRadiusSphere;
    private GameObject defenseActivationRadiusSphere;
    public bool teleportationTrigger = false;
    private float afterTimer = 15f;

    public string sceneToLoad;

    private List<GameObject> spawnedMonsters = new List<GameObject>();

    void Start()
    {
        InitialSpawnMonsters();
        if (defenseActivationRadiusSphere == null)
        {
            SpawnTransparentGreenSphere(transform.position, defenseActivationRadius);
        }
    }
    /*
     * OLD TYPE WORKING FUNCTION IF NEEDED
     * 
     * void InitialSpawnMonsters()
     {
         int monstersSpawned = 0;
         while (monstersSpawned < initialnumberOfMonsters)
         {
             Vector3 spawnPosition = GetRandomPositionWithinCircle(spawnRadius);
             int loopCnt = 0;
             while (!IsPositionValid(spawnPosition) && loopCnt < 30 ) 
             {
                 spawnPosition = GetRandomPositionWithinCircle(spawnRadius);
                 loopCnt++;
             }
             if(loopCnt==30 ) { break; }
             GameObject monsterPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Count)];
             GameObject newMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
             spawnedMonsters.Add(newMonster);
             monstersSpawned++;

         }
     }
    */

    void InitialSpawnMonsters()
    {
        currentEnemyPoints = 0;

        while (currentEnemyPoints < enemyPointLimit / 2)
        {
            int remainingPoints = (enemyPointLimit / 2) - currentEnemyPoints;

            GameObject selectedMonster = GetWeightedRandomMonster(spawnRollMin, spawnRollMax, remainingPoints, out int enemyValue);
            if (selectedMonster == null) break; // nothing fits

            Vector3 spawnPosition = GetValidSpawnPosition();
            if (spawnPosition == Vector3.zero) break; // couldn't find valid spawn point

            GameObject newMonster = Instantiate(selectedMonster, spawnPosition, Quaternion.identity);
            spawnedMonsters.Add(newMonster);
            currentEnemyPoints += enemyValue;

            AdjustRangeAfterSpawn(enemyValue);
        }
    }
    /*
     * OLD TYPE WORKING FUNCTION IF NEEDED
     * 
     * void SpawnMonsters()
    {
        CleanUpDestroyedMonsters();
        if (spawnedMonsters.Count < maxNumberOfMonsters)
        {
            float effectiveSpawnRadius = portalDefending ? spawnRadius + defendRadius : spawnRadius;
            Vector3 spawnPosition = GetRandomPositionWithinCircle(effectiveSpawnRadius);
            int loopCnt = 0;
            while (!IsPositionValid(spawnPosition) && loopCnt < 30)  
            {
                spawnPosition = GetRandomPositionWithinCircle(effectiveSpawnRadius);
                loopCnt++;
            }
            if (loopCnt < 30)
            {
                GameObject monsterPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Count)];
                GameObject newMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
                spawnedMonsters.Add(newMonster);
            }
            else
            {
                Debug.LogWarning("Failed to find a valid spawn position after 30 attempts.");
            }
        }
    }*/
    void SpawnMonsters()
    {
        CleanUpDestroyedMonsters();

        int remainingPoints = enemyPointLimit - currentEnemyPoints;

        GameObject selectedMonster = GetWeightedRandomMonster(spawnRollMin, spawnRollMax, remainingPoints, out int enemyValue);

        if (selectedMonster != null)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();
            if (spawnPosition != Vector3.zero)
            {
                GameObject newMonster = Instantiate(selectedMonster, spawnPosition, Quaternion.identity);
                spawnedMonsters.Add(newMonster);
                currentEnemyPoints += enemyValue;

                AdjustRangeAfterSpawn(enemyValue);
            }
        }
    }


   /* 
    * 2.0
    * GameObject GetWeightedRandomMonster(int minRange, int maxRange, out int selectedEnemyValue)
    {
        int rand = Random.Range(minRange, maxRange + 1);
        List<GameObject> candidates = new List<GameObject>();

        foreach (var prefab in monsterPrefabs)
        {
            OpponentBase opponent = prefab.GetComponent<OpponentBase>();
            int value = 1;
            if (opponent != null && opponent.enemyValue > 0) { value = opponent.enemyValue; }
            if ((value >= 5 && rand >= 80) || (value < 5 && rand < 80))
            {
                candidates.Add(prefab);
            }
        }
        if (candidates.Count == 0) { candidates = new List<GameObject>(monsterPrefabs); }

        GameObject selected = candidates[Random.Range(0, candidates.Count)];
        OpponentBase selectedOpponent = selected.GetComponent<OpponentBase>();
        selectedEnemyValue = (selectedOpponent != null) ? selectedOpponent.enemyValue : 1;

        return selected;
    }*/
    GameObject GetWeightedRandomMonster(int minRange, int maxRange, int maxValueLimit, out int selectedEnemyValue)
    {
        /*
         * version 3.0
         *
         */
        int rand = Random.Range(minRange, maxRange + 1);
        List<GameObject> candidates = new List<GameObject>();

        foreach (var prefab in monsterPrefabs)
        {
            OpponentBase opponent = prefab.GetComponent<OpponentBase>();
            int value = 1;
            if (opponent != null && opponent.enemyValue > 0) { value = opponent.enemyValue; }

            if (value <= maxValueLimit && ((value >= thresholdenemyValue && rand >= thresholdRandValue) || (value < thresholdenemyValue && rand < thresholdRandValue)))
            {
                candidates.Add(prefab);
            }
        }

        if (candidates.Count == 0)
        {
            foreach (var prefab in monsterPrefabs)
            {
                OpponentBase opponent = prefab.GetComponent<OpponentBase>();
                int value = 1;
                if (opponent != null && opponent.enemyValue > 0) { value = opponent.enemyValue; }

                if (value <= maxValueLimit)
                {
                    candidates.Add(prefab);
                }
            }
        }

        if (candidates.Count == 0)
        {
            selectedEnemyValue = 0;
            return null;
        }

        GameObject selected = candidates[Random.Range(0, candidates.Count)];
        OpponentBase selectedOpponent = selected.GetComponent<OpponentBase>();
        selectedEnemyValue = (selectedOpponent != null && selectedOpponent.enemyValue > 0) ? selectedOpponent.enemyValue : 1;

        return selected;
    }
    Vector3 GetRandomPositionWithinCircle(float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        if (portalDefending) //expanding while in defense phase
        {
            while (randomCircle.magnitude < defendRadius) 
            {
                randomCircle = Random.insideUnitCircle * radius;
            }
        }
        return new Vector3(randomCircle.x, 0, randomCircle.y) + transform.position;
    }


    Vector3 GetValidSpawnPosition()
    {
        float effectiveSpawnRadius = portalDefending ? spawnRadius + defendRadius : spawnRadius;
        int loopCnt = 0;
        Vector3 spawnPosition = GetRandomPositionWithinCircle(effectiveSpawnRadius);
        while (!IsPositionValid(spawnPosition) && loopCnt < 30)
        {
            spawnPosition = GetRandomPositionWithinCircle(effectiveSpawnRadius);
            loopCnt++;
        }
        return loopCnt >= 30 ? Vector3.zero : spawnPosition;
    }

    bool IsPositionValid(Vector3 position)
    {
        int combinedLayer = collisionLayer | playerCollisionLayer;
        Collider[] colliders = Physics.OverlapSphere(position, 1f, combinedLayer);
        return colliders.Length == 0;
    }

    /*public void CleanUpDestroyedMonsters()
    {
        spawnedMonsters.RemoveAll(monster => monster == null);
    }*/

    public void CleanUpDestroyedMonsters()
    {
        spawnedMonsters.RemoveAll(monster => monster == null);
        currentEnemyPoints = 0;

        foreach (var monster in spawnedMonsters)
        {
            if (monster != null)
            {
                OpponentBase opponent = monster.GetComponent<OpponentBase>();
                int value = (opponent != null && opponent.enemyValue > 0) ? opponent.enemyValue : 1;

                currentEnemyPoints += value;
            }
        }
    }

    void AdjustRangeAfterSpawn(int value)
    {
        if (value < thresholdenemyValue)
        {
            if (spawnRollMax < spawnRollMaxStart)
            {
                spawnRollMax = Mathf.Min(spawnRollMax + spawningStep, spawnRollMaxStart);
            }
            else if (spawnRollMin + spawningStep < spawnRollMax)
            {
                spawnRollMin += spawningStep;
            }
        }
        else // value >= thresholdenemyValue
        {
            if (spawnRollMin > spawnRollMinStart)
            {
                spawnRollMin = Mathf.Max(spawnRollMin - spawningStep, spawnRollMinStart);
            }
            else if (spawnRollMax - spawningStep > spawnRollMin)
            {
                spawnRollMax -= spawningStep;
            }
        }
    }

    public void SpawnTransparentRedSphere(Vector3 position, float radius)
    {
        if (transparentRed == null)
        {
            Debug.LogError("Material 'transparentRed.mat' not found");
            return;
        }

        defendRadiusSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        defendRadiusSphere.transform.position = position;
        defendRadiusSphere.transform.localScale = Vector3.one * radius * 2f;
        defendRadiusSphere.GetComponent<Renderer>().sharedMaterial = transparentRed;
        defendRadiusSphere.transform.SetParent(this.transform);
        Collider sphereCollider = defendRadiusSphere.GetComponent<Collider>();
        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
        }
    }

    public void SpawnTransparentGreenSphere(Vector3 position, float radius)
    {
        if (transparentGreen == null)
        {
            Debug.LogError("Material 'transparentGreen.mat' not found");
            return;
        }

        defenseActivationRadiusSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        defenseActivationRadiusSphere.transform.position = position;
        defenseActivationRadiusSphere.transform.localScale = Vector3.one * radius * 2f;
        defenseActivationRadiusSphere.GetComponent<Renderer>().sharedMaterial = transparentGreen;
        defenseActivationRadiusSphere.transform.SetParent(this.transform);
        Collider sphereCollider = defenseActivationRadiusSphere.GetComponent<Collider>();
        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
        }
    }
    private void Update()
    {
        if (defenseProgress < defenseMaxTime)//active until defense sequence is finished
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
                    defenseProgress += defenseUpValue;
                    oneSecTimer -= 1f;
                    Debug.Log("PROGRESS = " + defenseProgress);
                }
                else if (isPlayerInRange == false && oneSecTimer >= 1f)
                {
                    if (defenseProgress > 0f) { defenseProgress -= defenseDownValue; } else { defenseProgress = 0f; }//align to 0 if value too low
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
                SceneManager.LoadScene(sceneToLoad);
                Debug.Log("TELEPORT");
            }
        }
    }
   
}