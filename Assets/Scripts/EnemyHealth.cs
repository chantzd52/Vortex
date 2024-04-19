using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public int bulletShield = 0; // Shield for bullets
    public int laserShield = 0; // Shield for lasers

    public event Action OnDeath; // Event to signal death of the enemy

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

    void Die()
    {
        OnDeath?.Invoke(); // Notify all subscribers that this enemy has died
        Destroy(gameObject); // Destroy this enemy object
    }

    void Update()
    {
        // Here you could implement any additional logic needed for enemy health (like regeneration, etc.)
    }
}
