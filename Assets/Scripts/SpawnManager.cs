using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;  // Array to hold different enemy types
    public GameObject[] spawnPoints;   // Array of GameObjects as spawn points

    public float initialDelay = 2.0f;  // Initial delay before spawning starts
    public float waveInterval = 30.0f; // Time between waves
    private int waveNumber = 0;        // Track the number of waves

    void Start()
    {
        InvokeRepeating("SpawnWave", initialDelay, waveInterval);
    }

    void SpawnWave()
    {
        waveNumber++;
        int enemiesToSpawn = Mathf.FloorToInt(waveNumber * 1.5f); // Scale number of enemies with the wave number

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);  // Select random enemy type
        int spawnPointIndex = Random.Range(0, spawnPoints.Length); // Select random spawn point

        GameObject spawnPoint = spawnPoints[spawnPointIndex];
        Instantiate(enemyPrefabs[enemyIndex], spawnPoint.transform.position, Quaternion.identity);
    }
}
