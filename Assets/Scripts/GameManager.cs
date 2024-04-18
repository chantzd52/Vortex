using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Include this if you are manipulating UI elements directly

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;
    public Button respawnButton;
    public EnemySpawner enemySpawner;

    private GameObject currentPlayer;

    void Start()
    {
        StartGame();
        respawnButton.gameObject.SetActive(false);
        respawnButton.onClick.AddListener(RespawnPlayer);
    }

    void Update()
    {
        CheckPlayerStatus();
    }

    void StartGame()
    {
        if (currentPlayer == null)
        {
            SpawnPlayer();
        }
        enemySpawner.ResetSpawner(); // Start or restart enemy spawning
    }

    void CheckPlayerStatus()
    {
        if (currentPlayer == null && !respawnButton.gameObject.activeInHierarchy)
        {
            respawnButton.gameObject.SetActive(true);
        }
    }

    void SpawnPlayer()
    {
        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        //clear enemys
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        //clear bullets
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
        respawnButton.gameObject.SetActive(false);
    }

    public void RespawnPlayer()
    {
        if (currentPlayer == null)
        {
            SpawnPlayer();
            enemySpawner.ResetSpawner(); // Ensure enemy spawning continues
        }
    }
}