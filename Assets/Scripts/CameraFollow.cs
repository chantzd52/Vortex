using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // The player's transform
    public float smoothing = 5f;  // Smoothing factor for camera movement

    private Vector3 offset;  // Offset distance between the camera and the player

    void Start()
    {
        // Calculate the initial offset.
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        // Target position that the camera needs to reach
        Vector3 targetCamPos = target.position + offset;
        // Smoothly move the camera towards the target position
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}

