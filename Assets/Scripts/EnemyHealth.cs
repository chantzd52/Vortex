using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public int bulletShield = 0; // Shield for bullets
    public int laserShield = 0; // Shield for lasers
    public int xpValue = 10; // XP that this enemy provides when defeated

    public event Action OnDeath; // Event to signal death of the enemy

    
    [Serializable]
    public class DropItem
    {
        public GameObject item;
    }

    [Header("Drop Settings")]
    public DropItem[] dropItems; // Array of items that can drop
    public float dropChance = 0.25f; // Probability of dropping an item (25%).

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") && bulletShield > 0)
        {
            bulletShield--;
            Destroy(other.gameObject); // Destroy the bullet
        }
        else if (other.CompareTag("Laser") && laserShield > 0)
        {
            laserShield--;
            Destroy(other.gameObject); // Destroy the laser
        }
        else if ((other.CompareTag("Bullet") && bulletShield == 0) || (other.CompareTag("Laser") && laserShield == 0))
        {
            Die();
            Debug.Log("Ship hit! Explosion and destruction logic here.");
        }
    }

    public void Die()
    {
        Player player = FindObjectOfType<Player>(); // Find the player instance
        if (player != null)
        {
            player.AddXP(xpValue); // Give XP to the player
        }
        HandleDrop(); // Handle the drop of items
        Destroy(gameObject); // Destroy the enemy object
        OnDeath?.Invoke(); // Notify all subscribers that this enemy has died
        Destroy(gameObject); // Destroy this enemy object
    }

    private void HandleDrop()
{
    // If a item is to drop then drop a random item from the array
    if (dropItems.Length > 0 && UnityEngine.Random.value < dropChance)
    {
        int randomIndex = UnityEngine.Random.Range(0, dropItems.Length);
        DropItem item = dropItems[randomIndex];
        Instantiate(item.item, transform.position, Quaternion.identity);
    }
}

    void Update()
    {
        // Here you could implement any additional logic needed for enemy health (like regeneration, etc.)
    }
}
