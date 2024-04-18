using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // Assign the enemy prefab in the inspector
    public Transform[] enemySpawnPoints; // Assign spawn points for enemies
    public float spawnInterval = 5.0f; // Time in seconds between enemy spawns

    private float spawnTimer;

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        SpawnTimer();
    }

    private void SpawnTimer()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        if (enemySpawnPoints.Length > 0)
        {
            Transform spawnLocation = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
            Instantiate(enemyPrefab, spawnLocation.position, Quaternion.identity);
        }
    }

    public void ResetSpawner()
    {
        spawnTimer = spawnInterval; // Reset the timer to restart enemy spawning
    }
}
