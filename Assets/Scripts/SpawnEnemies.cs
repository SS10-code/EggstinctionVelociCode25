using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject Sentry;
    public GameObject Charger;
    public GameObject Gunner;

    [Header("Spawn Prefab")]
    public GameObject spawnEffectPrefab;

    public int minEnemies = 3;
    public int maxEnemies = 5;
    public float arenaWidth = 15.5f;
    public float arenaHeight = 8f;
    public float preSpawnDelay = 1.5f;
    public float minDistanceApart = 2f;

    void Start()
    {
        StartCoroutine(InstantiateEnemies());
    }

    IEnumerator InstantiateEnemies()
    {
        int totalCount = Random.Range(minEnemies, maxEnemies + 1);
        GameObject[] enemyTypes = { Sentry, Charger, Gunner };

        List<GameObject> toSpawn = new List<GameObject>();
        List<Vector2> spawnPos = new List<Vector2>();

        int safetyLimit = 1000;

        for (int i = 0; i < totalCount; i++)
        {
            GameObject selectedPrefab = enemyTypes[Random.Range(0, enemyTypes.Length)];
            toSpawn.Add(selectedPrefab);

            Vector2 candidatePos;
            int attempts = 0;

            do
            {
                float x = Random.Range(-arenaWidth / 2f, arenaWidth / 2f);
                float y = Random.Range(-arenaHeight / 2f, arenaHeight / 2f);
                candidatePos = new Vector2(x, y);
                attempts++;

                if (attempts > safetyLimit)
                    break;


            }
            while (!DistCheck(candidatePos, spawnPos));

            spawnPos.Add(candidatePos);

            if (spawnEffectPrefab != null)
            {
                Instantiate(spawnEffectPrefab, candidatePos, Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(preSpawnDelay);

        for (int i = 0; i < toSpawn.Count; i++)
        {
            Instantiate(toSpawn[i], spawnPos[i], Quaternion.identity);
        }
    }

    bool DistCheck(Vector2 newPos, List<Vector2> existing)
    {
        foreach (Vector2 pos in existing)
        {
            if (Vector2.Distance(newPos, pos) < minDistanceApart)
                return false;
        }
        return true;
    }
}