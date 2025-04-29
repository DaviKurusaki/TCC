using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    public int enemiesPerWave = 3;
    public float timeBetweenWaves = 5f;

    private int currentWave = 0;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    void Update()
    {
        // Se todos os inimigos foram derrotados, começa próxima wave
        currentEnemies.RemoveAll(enemy => enemy == null);
        if (currentEnemies.Count == 0 && !isSpawning)
        {
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;

        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;
        FindObjectOfType<UIManager>().SetWave(currentWave);
        enemiesPerWave += 2; // Aumenta dificuldade por wave

        for (int i = 0; i < enemiesPerWave; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            currentEnemies.Add(enemy);
    
        }
       

        isSpawning = false;
    }
}
