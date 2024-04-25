using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public Vector3 movementDirection = Vector3.down;
    public float moveDistance = 5.0f;
    public float moveSpeed = 2.0f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isActive = false;
    private bool movingToTarget = true;

    public AudioSource audioSource;
    public AudioClip laserSound;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + movementDirection.normalized * moveDistance;
        gameObject.SetActive(false);  // Start with the laser deactivated
    }

    void Update()
    {
        if (isActive)
        {
            MoveLaser();
        }
    }

    void MoveLaser()
    {
        if (movingToTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                movingToTarget = false;  // Switch direction to move back
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPosition) < 0.01f)
            {
                movingToTarget = true;  // Switch direction to move forward
            }
        }
    }

    public void ActivateLaser()
    {
        isActive = true;
        gameObject.SetActive(true);  // Make the laser visible and active
        audioSource.PlayOneShot(laserSound);
    }

    public void DeactivateLaser()
    {
        isActive = false;
        gameObject.SetActive(false);  // Hide the laser and stop it from updating
        ResetPosition();
        audioSource.Stop();
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        movingToTarget = true;  // Ready to move towards the target when activated again
    }
}