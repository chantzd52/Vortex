using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public int bulletShield = 0; // Shield for bullets
    public int laserShield = 0; // Shield for lasers

    public bool OntouchKillEnemy = false; // Kill enemy on touch

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") && bulletShield > 0)
        {
            bulletShield--;
        }
        
        else if (other.CompareTag("Laser") && laserShield > 0)
        {
            laserShield--;
        }
        else if (other.CompareTag("Bullet") && bulletShield == 0)
        {
            PlayerDead();
        }
        else if (other.CompareTag("Laser") && laserShield == 0)
        {
            PlayerDead();
        }

        else if (other.CompareTag("Enemy") && OntouchKillEnemy) // This needs to be changed so that when touching a enemy the enemy dies.
        {
            Destroy(other.gameObject);
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
        // Add any regular update logic here
    }
}


