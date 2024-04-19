using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyWave
    {
        public GameObject[] levelEnemies;
        public float spawnInterval = 2.0f;
        public int maxEnemies = 10;
        public float waveDuration = 60.0f; // Duration in seconds
    }

    [System.Serializable]
    public class EventWave
    {
        public GameObject eventPrefab;
        public float duration = 30.0f; // Duration in seconds
    }

    public EnemyWave[] waves; // Configurable in inspector for each level
    public EventWave[] events; // Configurable in inspector for each event level
    public Transform[] spawnPoints; // Assign spawn points for enemies

    private int currentWave = 0;
    private float waveTimer;
    private float spawnTimer;
    private int enemiesSpawned = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        StartWave();
    }

    void Update()
    {
        if (currentWave < waves.Length)
        {
            HandleWave();
        }
    }

    void StartWave()
    {
        Debug.Log("Wave " + (currentWave + 1) + " started!");
        waveTimer = waves[currentWave].waveDuration;
        spawnTimer = waves[currentWave].spawnInterval;
        enemiesSpawned = 0;
        activeEnemies.Clear();
    }

    void HandleWave()
    {
        waveTimer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0 && enemiesSpawned < waves[currentWave].maxEnemies)
        {
            SpawnEnemy();
            spawnTimer = waves[currentWave].spawnInterval;
            enemiesSpawned++;
        }

        if ((waveTimer <= 0 || enemiesSpawned >= waves[currentWave].maxEnemies) && activeEnemies.Count == 0)
        {
            currentWave++;
            if (currentWave < waves.Length)
            {
                StartWave();
            }
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length > 0)
    {
        Transform spawnLocation = spawnPoints[Random.Range(0, spawnPoints.Length)];
        int enemyIndex = Random.Range(0, waves[currentWave].levelEnemies.Length);
        GameObject enemy = Instantiate(waves[currentWave].levelEnemies[enemyIndex], spawnLocation.position, Quaternion.identity);
        activeEnemies.Add(enemy);
        enemy.GetComponent<EnemyHealth>().OnDeath += () => {
            activeEnemies.Remove(enemy); // Remove from list when the enemy dies
        };
    }

    }

    public void ResetSpawner()
    {
        currentWave = 0;
        StartWave(); // Restart from the first wave
    }
}