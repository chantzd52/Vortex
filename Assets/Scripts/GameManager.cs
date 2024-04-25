using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;
    public Button respawnButton;
    public EnemySpawner enemySpawner;

    public Blink Blink;

    public PowerUpSelection powerUpSelection;

    public TextMeshProUGUI timerText; // Reference to a TextMeshProUGUI component to display the timer

    private float startTime;
    private bool timerActive = false;

    private GameObject currentPlayer;


    void Start()
    {
        
        respawnButton.gameObject.SetActive(false);
        respawnButton.onClick.AddListener(RespawnPlayer);
        
    }

    void Update()
    {
        CheckPlayerStatus();
        UpdateTimer();
    }

    void StartGame()
    {
        if (currentPlayer == null)
        {
            SpawnPlayer();
            StartTimer();
            
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
        //clear lasers
        GameObject[] lasers = GameObject.FindGameObjectsWithTag("Laser");
        foreach (GameObject laser in lasers)
        {
            Destroy(laser);
        }
        //clear powerups
        GameObject[] powerups = GameObject.FindGameObjectsWithTag("AddBulletShieldPowerUp");
        foreach (GameObject powerup in powerups)
        {
            Destroy(powerup);
        }
        GameObject[] powerups2 = GameObject.FindGameObjectsWithTag("AddLazerShieldPowerUp");
        foreach (GameObject powerup in powerups2)
        {
            Destroy(powerup);
        }

        respawnButton.gameObject.SetActive(false);
    }

        public void PlayerDied()
    {

        StopTimer();
        powerUpSelection.HidePowerUpSelection();
        respawnButton.gameObject.SetActive(true);
        
        // Handle other game over logic here
    }

        private void StartTimer()
    {
        startTime = Time.time;
        timerActive = true;
    }

    private void StopTimer()
    {
        timerActive = false;
    }

    private void ResetTimer()
    {
        StartTimer(); // Restart timer
        timerText.text = FormatTime(0); // Reset the display to zero
    }

    private void UpdateTimer()
    {
        if (timerActive)
        {
            float timeElapsed = Time.time - startTime;
            timerText.text = FormatTime(timeElapsed);
        }
    }

    private string FormatTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }

    public void RespawnPlayer()
    {
        if (currentPlayer == null)
        {
            SpawnPlayer();
            enemySpawner.ResetSpawner(); // Ensure enemy spawning continues
            enemySpawner.ResetEvents(); // Reset events
            ResetTimer();
            
        }
    }
}