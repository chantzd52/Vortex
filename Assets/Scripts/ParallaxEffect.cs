using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float parallaxMultiplier = 0.02f; // Reduced multiplier for subtle effect

    private Transform playerTransform;
    private Vector3 lastPlayerPosition;
    private Vector3 initialPosition; // Store the initial position of the background

    void Start()
    {
        initialPosition = transform.position; // Save the initial position

        // Initialize player position tracking if the player exists at start
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            lastPlayerPosition = playerTransform.position;
        }
    }

    void Update()
    {
        // Ensure the player is assigned
        if (playerTransform == null)
        {
            FindPlayer();
            return; // Exit if no player is found
        }

        if (lastPlayerPosition != null) {
            Vector3 deltaMovement = playerTransform.position - lastPlayerPosition;
            float parallaxEffect = deltaMovement.x * parallaxMultiplier;

            // Apply the parallax effect based on the initial position
            transform.position = initialPosition + new Vector3(parallaxEffect, 0, 0);
            lastPlayerPosition = playerTransform.position;
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            lastPlayerPosition = playerTransform.position;
        }
    }
}
