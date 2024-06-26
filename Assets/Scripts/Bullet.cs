using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public GameObject explosionPrefab;
    private GameObject shooter;  // Reference to the object that shot the bullet

    public AudioSource audioSource; // The AudioSource component
    public AudioClip bullet; // The powerup sound clip


    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
        //play sound of bullet shooting
        audioSource.PlayOneShot(bullet);

    }

    // Call this method when creating the bullet to set its shooter
    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        // Destroy the bullet when it hits a boundary
        if (other.CompareTag("Boundary"))
        {
            Destroy(gameObject);
            return;
        }

        // The bullet hits player and gets destroyed
        if (other.CompareTag("Player"))
        {
            TriggerHitEffects(other.transform.position);
            Destroy(gameObject);
        }

        // The bullet harms other enemies immediately
        if (other.CompareTag("Enemy"))
        {
            TriggerHitEffects(other.transform.position);
            Destroy(gameObject);
        }
    }

    // Handle the explosion and destruction of the hit object
    private void TriggerHitEffects(Vector3 position)
    {
        
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, position, Quaternion.identity);

        Destroy(gameObject);  // Destroy the bullet
    }
}