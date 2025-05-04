using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyType
    {
        public GameObject prefab;
        [Range(0f, 1f)]
        public float spawnChance;
    }

    [Header("Objetos de Recompensa")]
    public GameObject FirePickup;
    public GameObject IcePickup;
    public GameObject LightningPickup;


    [Header("Partícula de Spawn")]
    public GameObject efeitoParticulas;
    public float delay = 2f;

    [Header("Tipos de Inimigos")]
    public EnemyType[] enemyTypes;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Configuração da Wave")]
    public int enemiesPerWave = 3;
    public float timeBetweenWaves = 5f;
    public int totalWaves = 5;

    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    private int currentWave = 0;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private bool isSpawning = false;
    private bool bossSpawned = false;

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    void Update()
    {
        if (currentWave == 2)
        {
            FirePickup.SetActive(true);
        }
        if (currentWave == 3)
        {
            IcePickup.SetActive(true);
        }
        if (currentWave == 4)
        {
            LightningPickup.SetActive(true);
        }

        currentEnemies.RemoveAll(enemy => enemy == null);

        if (currentEnemies.Count == 0 && !isSpawning)
        {
            if (currentWave < totalWaves)
            {
                StartCoroutine(SpawnWave());
            }
            else if (!bossSpawned)
            {
                SpawnBoss();
            }
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;

        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;
        FindObjectOfType<UIManager>().SetWave(currentWave);

        enemiesPerWave += 2;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject selectedEnemy = GetRandomEnemy();

            if (selectedEnemy != null)
            {
                GameObject efeito = Instantiate(efeitoParticulas, spawnPoint.position, Quaternion.identity);
                Destroy(efeito, delay);

                yield return new WaitForSeconds(delay);

                GameObject enemy = Instantiate(selectedEnemy, spawnPoint.position, Quaternion.identity);
                currentEnemies.Add(enemy);
            }
        }

        

        isSpawning = false;
    }

    void SpawnBoss()
    {
        if (bossPrefab != null && bossSpawnPoint != null)
        {
            GameObject efeito = Instantiate(efeitoParticulas, bossSpawnPoint.position, Quaternion.identity);
            Destroy(efeito, delay);

            StartCoroutine(SpawnBossWithDelay());
        }
        else
        {
            Debug.LogError("[Spawner] BossPrefab ou BossSpawnPoint não configurados!");
        }

        bossSpawned = true;
    }

    IEnumerator SpawnBossWithDelay()
    {
        yield return new WaitForSeconds(delay);
        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        currentEnemies.Add(boss);
        Debug.Log("Boss spawned!");
    }

    GameObject GetRandomEnemy()
    {
        float rand = Random.value;
        float cumulative = 0f;

        foreach (var enemyType in enemyTypes)
        {
            cumulative += enemyType.spawnChance;
            if (rand <= cumulative)
            {
                return enemyType.prefab;
            }
        }

        return enemyTypes.Length > 0 ? enemyTypes[enemyTypes.Length - 1].prefab : null;
    }
}
