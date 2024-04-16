using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public float thrustPower = 5f;
    public float rotationSpeed = 200f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        ThrustForward();
        Rotate();
    }

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
}
