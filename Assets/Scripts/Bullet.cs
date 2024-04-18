using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public GameObject explosionPrefab;
    private GameObject shooter;  // Reference to the object that shot the bullet

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
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

        // The bullet can always harm the player
        if (other.CompareTag("Player"))
        {
            TriggerHitEffects(other.transform.position);
            Destroy(other.gameObject);
            //Destroy object with tag portal
            GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
            foreach (GameObject portal in portals)
            {
                Destroy(portal);
            }
        }

        // The bullet harms other enemies immediately
        if (other.CompareTag("Enemy"))
        {
            TriggerHitEffects(other.transform.position);
            Destroy(other.gameObject);
        }
    }

    // Handle the explosion and destruction of the hit object
    private void TriggerHitEffects(Vector3 position)
    {
        Debug.Log("Ship hit! Explosion and destruction logic here.");
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, position, Quaternion.identity);

        Destroy(gameObject);  // Destroy the bullet
    }
}