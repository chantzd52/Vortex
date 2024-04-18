using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab; // Assign the player prefab in the inspector
    public GameObject enemyPrefab;  // Assign the enemy prefab in the inspector
    public Transform spawnPoint;    // Assign the spawn point for player
    public Transform[] enemySpawnPoints; // Assign spawn points for enemies

    public float spawnInterval = 5.0f; // Time in seconds between enemy spawns

    private GameObject currentPlayer;
    private float spawnTimer;

    void Start()
    {
        StartGame();
        spawnTimer = spawnInterval; // Initialize the spawn timer
    }

    void Update()
    {
        CheckPlayerStatus();
        EnemySpawnTimer();
    }

    void StartGame()
    {
        if (currentPlayer == null)
        {
            SpawnPlayer();
        }
    }

    void CheckPlayerStatus()
    {
        if (currentPlayer == null)
        {
            // Player has died
            RespawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
    }

    void RespawnPlayer()
    {
        // Optionally add a delay or some effect
        Invoke("SpawnPlayer", 2.0f); // Wait 2 seconds before respawning
    }

    void EnemySpawnTimer()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (enemySpawnPoints.Length > 0)
        {
            Transform spawnLocation = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
            Instantiate(enemyPrefab, spawnLocation.position, Quaternion.identity);
        }
    }
}