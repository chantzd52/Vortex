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
    private float elapsedTime;

    private float startTime;
    private bool timerActive = false;

    public TextMeshProUGUI highScore1Text;
    public TextMeshProUGUI highScore2Text;
    public TextMeshProUGUI highScore3Text;

    public GameObject highScoresPanel;

    private GameObject currentPlayer;


    void Start()
    {
        
        respawnButton.gameObject.SetActive(false);
        respawnButton.onClick.AddListener(RespawnPlayer);
        DisplayHighScores();
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
            powerUpSelection.ResetPowerUpCounts();
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

        highScoresPanel.SetActive(false); // Hide the high scores panel
        respawnButton.gameObject.SetActive(false);
        
    }

        public void PlayerDied()
    {

        StopTimer();
        CheckForHighScore(elapsedTime);
        powerUpSelection.HidePowerUpSelection();
        highScoresPanel.SetActive(true); // Show the high scores panel
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
        elapsedTime = Time.time - startTime;
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
        return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    private void CheckForHighScore(float score)
    {
        float highScore1 = PlayerPrefs.GetFloat("HighScore1", 0);
        float highScore2 = PlayerPrefs.GetFloat("HighScore2", 0);
        float highScore3 = PlayerPrefs.GetFloat("HighScore3", 0);

        if (score > highScore1)
        {
            PlayerPrefs.SetFloat("HighScore3", highScore2);
            PlayerPrefs.SetFloat("HighScore2", highScore1);
            PlayerPrefs.SetFloat("HighScore1", score);
        }
        else if (score > highScore2)
        {
            PlayerPrefs.SetFloat("HighScore3", highScore2);
            PlayerPrefs.SetFloat("HighScore2", score);
        }
        else if (score > highScore3)
        {
            PlayerPrefs.SetFloat("HighScore3", score);
        }

        DisplayHighScores();
    }

     private void DisplayHighScores()
    {
        highScore1Text.text = "1st: " + FormatTime(PlayerPrefs.GetFloat("HighScore1", 0));
        highScore2Text.text = "2nd: " + FormatTime(PlayerPrefs.GetFloat("HighScore2", 0));
        highScore3Text.text = "3rd: " + FormatTime(PlayerPrefs.GetFloat("HighScore3", 0));
    }

   

    public void RespawnPlayer()
    {
        if (currentPlayer == null)
        {
            SpawnPlayer();
            enemySpawner.ResetSpawner(); // Ensure enemy spawning continues
            enemySpawner.ResetEvents(); // Reset events
            ResetTimer();
            powerUpSelection.ResetPowerUpCounts();
            
        }
    }
}