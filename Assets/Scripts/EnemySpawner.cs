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
        public float duration = 30.0f;  // Duration in seconds
        public bool isActive = false;  // Is the event active (like a laser) or does it spawn something
        public Transform targetTransform;  // Target transform for spawning or activating the event
    }

    public EnemyWave[] waves; // Configurable in inspector for each level
    public EventWave[] events; // Configurable in inspector for each event level
    public Transform[] spawnPoints; // Assign spawn points for enemies

    private int currentWave = 0;
    private float waveTimer;
    private float spawnTimer;
    private int enemiesSpawned = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    public float eventInterval = 120f;  // Time in seconds between each event
    private float eventTimer;

    void Start()
    {
        StartWave();
        eventTimer = eventInterval;  // Initialize the event timer
    }

    void Update()
    {
        if (currentWave < waves.Length)
        {
            HandleWave();
        }
        HandleEvents();
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

    // Spawn new enemies at defined intervals until the maximum count for the current wave is reached
    if (spawnTimer <= 0 && enemiesSpawned < waves[currentWave].maxEnemies)
    {
        SpawnEnemy();
        spawnTimer = waves[currentWave].spawnInterval;
        enemiesSpawned++;
    }

    // Check if the wave timer has expired or all enemies for this wave have been spawned
    if (waveTimer <= 0 || (enemiesSpawned >= waves[currentWave].maxEnemies && activeEnemies.Count == 0))
    {
        // Proceed to the next wave if the timer runs out or all enemies are defeated
        currentWave++;
        if (currentWave < waves.Length)
        {
            StartWave();
        }
        else
        {
            Debug.Log("All waves complete.");
            // Optionally, implement what happens when all waves are complete (e.g., game victory or loop the waves).
        }
    }
}

void HandleEvents()
{
    eventTimer -= Time.deltaTime;

    if (eventTimer <= 0)
    {
        TriggerEvent();
        eventTimer = eventInterval;  // Reset the event timer
    }
}

void TriggerEvent()
{
    if (events.Length > 0)
    {
        EventWave eventWave = events[Random.Range(0, events.Length)];  // Choose a random event

        if (eventWave.isActive)
        {
            // Check if the event prefab is a laser and manage its activation
            LaserController laserController = eventWave.eventPrefab.GetComponent<LaserController>();
            if (laserController != null)
            {
                if (!eventWave.eventPrefab.activeInHierarchy)  // Ensure we only activate it if it's not already active
                {
                    eventWave.eventPrefab.SetActive(true);
                    laserController.ActivateLaser();  // Use a method in LaserController to handle its activation
                    StartCoroutine(DeactivateAfterDuration(eventWave.eventPrefab, eventWave.duration, laserController));
                }
            }
        }
        else
        {
            // Spawn the event object
            if (eventWave.targetTransform != null)
            {
                Instantiate(eventWave.eventPrefab, eventWave.targetTransform.position, Quaternion.identity);
            }
        }
    }
}

IEnumerator DeactivateAfterDuration(GameObject obj, float duration, LaserController laserController = null)
{
    yield return new WaitForSeconds(duration);
    if (laserController != null)
    {
        laserController.DeactivateLaser();  // Properly deactivate using the laser's control method
    }
    else
    {
        obj.SetActive(false);  // Deactivate after the specified duration
    }
}

public void ResetEvents()
{
    foreach (EventWave eventWave in events)
    {
        if (eventWave.isActive && eventWave.eventPrefab.activeInHierarchy)
        {
            eventWave.eventPrefab.SetActive(false);
            LaserController laserController = eventWave.eventPrefab.GetComponent<LaserController>();
            if (laserController != null)
            {
                laserController.ResetPosition();  // Assuming there's a method to reset the position
            }
        }
    }
    eventTimer = eventInterval;  // Reset the event timer to start firing events again
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