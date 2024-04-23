using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public int bulletShield = 0; // Shield for bullets
    public int laserShield = 0; // Shield for lasers
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

    public delegate void LevelUpAction(); // Event to trigger when player levels up
    public event LevelUpAction OnLevelUp;
    public AudioSource audioSource; // The AudioSource component
    public AudioClip Powerup; // The powerup sound clip

    void Start()
    {
        OnLevelUp += LevelUpHandler;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

     void OnDestroy()
    {
        OnLevelUp -= LevelUpHandler; // Unsubscribe to prevent memory leaks
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvincible) return; // Skip all checks if invincible

        if (other.CompareTag("Bullet") && bulletShield > 0)
        {
            bulletShield--;
        }
        else if (other.CompareTag("Laser") && laserShield > 0 || other.CompareTag("LaserEvent") && laserShield > 0)
        {
            laserShield--;
        }
        else if (other.CompareTag("Bullet") && bulletShield == 0 || other.CompareTag("Laser") && laserShield == 0 || other.CompareTag("LaserEvent") && laserShield == 0)
        {
            PlayerDead();
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

}


