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
    public GameObject[] bossPrefabs;
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
        // Ativa pickups por wave
        if (currentWave == 2) FirePickup.SetActive(true);
        if (currentWave == 3) IcePickup.SetActive(true);
        if (currentWave == 4) LightningPickup.SetActive(true);

        currentEnemies.RemoveAll(enemy => enemy == null);

        if (currentEnemies.Count == 0 && !isSpawning)
        {
            if (!bossSpawned)
            {
                SpawnBoss(); // Spawna o boss da wave atual
            }
            else if (currentWave < totalWaves)
            {
                StartCoroutine(SpawnWave()); // Vai pra próxima wave
            }
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;
        bossSpawned = false;

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
        int bossIndex = currentWave - 1;

        if (bossIndex < bossPrefabs.Length && bossSpawnPoint != null)
        {
            GameObject bossToSpawn = bossPrefabs[bossIndex];

            if (bossToSpawn != null)
            {
                GameObject efeito = Instantiate(efeitoParticulas, bossSpawnPoint.position, Quaternion.identity);
                Destroy(efeito, delay);
                StartCoroutine(SpawnBossWithDelay(bossToSpawn));
            }
        }
        else
        {
            Debug.LogWarning($"[Spawner] Nenhum boss configurado para a wave {currentWave}.");
        }

        bossSpawned = true;
    }

    IEnumerator SpawnBossWithDelay(GameObject bossPrefab)
    {
        yield return new WaitForSeconds(delay);
        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        currentEnemies.Add(boss);
        Debug.Log($"Boss da Wave {currentWave} spawned!");
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
