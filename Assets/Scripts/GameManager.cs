using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;
    public Button respawnButton;
    public EnemySpawner enemySpawner;

    public PowerUpSelection powerUpSelection;
    

    private GameObject currentPlayer;

    void Start()
    {
        
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
        powerUpSelection.HidePowerUpSelection();
    }

    void CheckPlayerStatus()
    {
        if (currentPlayer == null && !respawnButton.gameObject.activeInHierarchy)
        {
            PlayerDied();
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

        public void PlayerDied()
    {
        powerUpSelection.HidePowerUpSelection();
        respawnButton.gameObject.SetActive(true);
        // Handle other game over logic here
    }

    public void RespawnPlayer()
    {
        if (currentPlayer == null)
        {
            SpawnPlayer();
            enemySpawner.ResetSpawner(); // Ensure enemy spawning continues
            enemySpawner.ResetEvents(); // Reset events
        }
    }
}