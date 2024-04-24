using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{

    public int maxBulletShieldSlots = 1; // Maximum slots for bullet shields
    public int currentBulletShields = 0; // Current number of bullet shields
    public int maxLaserShieldSlots = 1; // Maximum slots for laser shields
    public int currentLaserShields = 0; // Current number of laser shields
    public bool isInvincible = false; // Flag to check if the player is invincible
    public float invincibleDuration = 3.0f; // Duration of invincibility in seconds
    private float invincibleTimer; // Timer to track invincibility duration
    public float blinkRate = 0.1f; // How often the player blinks when invincible
    private float blinkTimer; // Timer to control the blinking
    public bool OntouchKillEnemy = false; // Kill enemy on touch
    public int currentXP = 0; // Player's current XP
    public int xpForNextLevel = 600; // XP needed for the next level
    public PowerUpSelection powerUpSelection;
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private PlayerUIManager uiManager; // Reference to the PlayerUIManager component


    public delegate void LevelUpAction(); // Event to trigger when player levels up
    public event LevelUpAction OnLevelUp;
    public AudioSource audioSource; // The AudioSource component
    public AudioClip Powerup; // The powerup sound clip

    void Start()
    {
        uiManager = FindObjectOfType<PlayerUIManager>(); // Find the UI manager in the scene
        UpdateUI(); // Update UI at start to initialize shield display correctly
        OnLevelUp += LevelUpHandler;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

       

     void OnDestroy()
    {
        OnLevelUp -= LevelUpHandler; // Unsubscribe to prevent memory leaks
    }





    void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvincible){
            if (other.CompareTag("AddBulletShieldPowerUp"))
            {
                Destroy(other.gameObject); // Destroy the power-up object
                AddBulletShield(); // Increase bullet shield count
            }
            else if (other.CompareTag("AddLazerShieldPowerUp"))
            {
                Destroy(other.gameObject); // Destroy the power-up object
                AddLaserShield(); // Increase laser shield count
        } return; // Skip all checks if invincible
        }

        if (other.CompareTag("Bullet") && currentBulletShields > 0)
        {
            currentBulletShields--;
            UpdateUI();
        }
        else if (other.CompareTag("Laser") && currentLaserShields > 0 || other.CompareTag("LaserEvent") && currentLaserShields > 0)
        {
            currentLaserShields--;
            UpdateUI();
        }
        else if (other.CompareTag("Bullet") && currentBulletShields == 0 || other.CompareTag("Laser") && currentLaserShields == 0 || other.CompareTag("LaserEvent") && currentLaserShields == 0)
        {
            PlayerDead();
        }
        else if (other.CompareTag("AddBulletShieldPowerUp"))
        {
            Destroy(other.gameObject); // Destroy the power-up object
            AddBulletShield(); // Increase bullet shield count
        }
        else if (other.CompareTag("AddLazerShieldPowerUp"))
        {
            Destroy(other.gameObject); // Destroy the power-up object
            AddLaserShield(); // Increase laser shield count
        }
        else if (other.CompareTag("Enemy") && OntouchKillEnemy)
        {
            Destroy(other.gameObject);  // Destroy the enemy on touch if the flag is true
        }
        else if (other.CompareTag("Enemy") && !OntouchKillEnemy)
        {
            PlayerDead();
        }
    }

    void PlayerDead()
    {
        Debug.Log("Player dead");
        DestroyEnemiesAndPortals();
        Destroy(gameObject); // Destroy player object
    }

    void DestroyEnemiesAndPortals()
    {
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
        foreach (var portal in GameObject.FindGameObjectsWithTag("Portal"))
        {
            Destroy(portal);
        }
    }

        void Update()
    {
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            blinkTimer -= Time.deltaTime;

            if (blinkTimer <= 0)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, spriteRenderer.color.a == 1.0f ? 0.5f : 1.0f);
                blinkTimer = blinkRate;
            }

            if (invincibleTimer <= 0)
            {
                isInvincible = false;
                spriteRenderer.color = new Color(1f, 1f, 1f, 1.0f); // Reset the sprite color to fully opaque
            }
        }
    }

    public void AddBulletShieldSlot()
    {
         if (maxBulletShieldSlots < 4) // Ensure we do not exceed four slots
        {
            maxBulletShieldSlots++;
            UpdateUI();
           
        }
    }

    public void AddLaserShieldSlot()
    {
        if(maxLaserShieldSlots < 4) // Ensure we do not exceed four slots
        {
            maxLaserShieldSlots++;
            UpdateUI();
        }
    }

   

    public void AddBulletShield()
    {
        if (currentBulletShields < maxBulletShieldSlots)
        {
            currentBulletShields++;
            UpdateUI();
        }
    }

    public void AddLaserShield()
    {
        if (currentLaserShields < maxLaserShieldSlots)
        {
            currentLaserShields++;
            UpdateUI();
        }
    }

    public void IncreaseBulletShield()
{
    AddBulletShield(); // Increase bullet shield count
    UpdateUI();
    Debug.Log("Bullet shield increased to: " + currentBulletShields);
}

public void IncreaseLaserShield()
{
    currentLaserShields++;  // Increase laser shield count
    UpdateUI();
    Debug.Log("Laser shield increased to: " + currentLaserShields);
}

private void UpdateUI()
{
    if (uiManager != null)
    {
        uiManager.UpdateBulletShieldIcons(currentBulletShields, maxBulletShieldSlots);
        uiManager.UpdateLaserShieldIcons(currentLaserShields, maxLaserShieldSlots);
    }
}

public void IncreaseSpeed()
{
    // Assuming you have a speed property
    // Increase player's movement speed
}

public void IncreaseTurnSpeed()
{
    // Assuming you have a turn speed property
    // Increase player's turning speed
}

    // Call this method to add XP to the player
    public void AddXP(int xpAmount)
    {
        currentXP += xpAmount;
        CheckForLevelUp();
    }

    // Check if the player has enough XP to level up
    private void CheckForLevelUp()
    {
        if (currentXP >= xpForNextLevel)
        {
            audioSource.PlayOneShot(Powerup);  // Play the level up sound clip
            currentXP -= xpForNextLevel; // Reset XP or keep track of excess
            OnLevelUp?.Invoke(); // Trigger any level up events
            Debug.Log("Player leveled up!");  // Implement additional level up logic here, e.g., increase stats, show level up UI
            
        }
    }

    private void LevelUpHandler()
    {
        PowerUpSelection powerUpSelection = FindObjectOfType<PowerUpSelection>();
        if (powerUpSelection != null)
        {
            powerUpSelection.ShowPowerUpSelection();
        }
    }

    public void BecomeInvincible()
    {
        isInvincible = true;
        invincibleTimer = invincibleDuration;
         blinkTimer = blinkRate; // Reset the blink timer
    }


}


