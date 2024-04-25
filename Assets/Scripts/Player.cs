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

    public SpaceshipController spaceShipController;  // get access to SpaceShipController

    public PortalController portalController;  // get access to PortalController


    public delegate void LevelUpAction(); // Event to trigger when player levels up
    public event LevelUpAction OnLevelUp;
    public AudioSource audioSource; // The AudioSource component
    public AudioClip Powerup; // The powerup sound clip

    public AudioSource hurtSource; // The AudioSource component
    public AudioClip hurt; // The hurt sound clip

    public float thrustPower = 5f;
    public float rotationSpeed = 200f;

    private Rigidbody2D rb;

    private void ThrustForward()
    {
        if (Input.GetKey(KeyCode.W))
        {
            // Apply a forward force to the Rigidbody in the direction it's facing
            rb.AddForce(transform.up * thrustPower);
        }
    }

    private void Rotate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            // Rotate counterclockwise
            rb.angularVelocity = rotationSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Rotate clockwise
            rb.angularVelocity = -rotationSpeed;
        }
        else
        {
            // No rotation input, stop rotating
            rb.angularVelocity = 0;
        }
    }

    public void IncreaseThrustPower()
    {
        thrustPower += 2;
    }

    public void IncreaseTurnSpeed()
    {
        rotationSpeed += 20;
    }

    public void DecreaseShipSize()
    {
        //Make th eplayer object scale -1
        transform.localScale -= new Vector3(1, 1, 1);
    }

    void Start()
    {
        uiManager = FindObjectOfType<PlayerUIManager>(); // Find the UI manager in the scene
        UpdateUI(); // Update UI at start to initialize shield display correctly
        OnLevelUp += LevelUpHandler;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
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
            hurtSource.PlayOneShot(hurt);  // Play the hurt sound clip
        }
        else if (other.CompareTag("Laser") && currentLaserShields > 0 || other.CompareTag("LaserEvent") && currentLaserShields > 0)
        {
            currentLaserShields--;
            UpdateUI();
            hurtSource.PlayOneShot(hurt);  // Play the hurt sound clip
        }
        else if (other.CompareTag("Bullet") && currentBulletShields == 0 || other.CompareTag("Laser") && currentLaserShields == 0 || other.CompareTag("LaserEvent") && currentLaserShields == 0 || other.CompareTag("Enemy") && currentBulletShields == 0)
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
        else if (other.CompareTag("Enemy") && currentBulletShields > 0)
        {
        {
            currentBulletShields--;
            UpdateUI();
            hurtSource.PlayOneShot(hurt); 
            other.GetComponent<EnemyHealth>().Die();
        }
    }
    }

    void PlayerDead()
    {
        Debug.Log("Player dead");
        hurtSource.PlayOneShot(hurt);  // Play the hurt sound clip
        StartCoroutine(DestroyAfterSound());
    }

    IEnumerator DestroyAfterSound()
{
    yield return new WaitForSeconds(hurt.length);  // Wait for the length of the hurt sound
    DestroyEnemiesAndPortals();
    Destroy(gameObject); // Destroy player object after the sound has played
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

    void FixedUpdate()
    {
        ThrustForward();
        Rotate();
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

public void IncreasePortalSize()
{
    // Find the objects taged portal and increase Scale by 1
    foreach (var portal in GameObject.FindGameObjectsWithTag("Portal"))
    {
        portal.transform.localScale += new Vector3(1, 1, 1);
    }

    Debug.Log("Portal size increased!");
}

private void UpdateUI()
{
    if (uiManager != null)
    {
        uiManager.UpdateBulletShieldIcons(currentBulletShields, maxBulletShieldSlots);
        uiManager.UpdateLaserShieldIcons(currentLaserShields, maxLaserShieldSlots);
    }
}






public void increaseBlender()
{
    portalController.IncreaseBlender();  //use portal controller to increase blender
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
            xpForNextLevel += 200; // Increase the XP needed for the next level
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


