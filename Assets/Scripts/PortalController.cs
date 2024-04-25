using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public GameObject bluePortalPrefab;
    public GameObject redPortalPrefab;
    private GameObject bluePortal;
    private GameObject redPortal;
    private Vector3 mousePosition;
    private Camera mainCamera;

    private Dictionary<GameObject, Vector2> originalVelocities = new Dictionary<GameObject, Vector2>();
    private Dictionary<GameObject, float> teleportCooldowns = new Dictionary<GameObject, float>();
    public float teleportCooldownDuration = 0.05f; // seconds
    public BoxCollider2D allowedPlacementArea;

    public float blenderOdds = 0f; // Percentage chance to kill the object instead of teleporting

    void Start()
    {
        mainCamera = Camera.main;
        allowedPlacementArea = GameObject.FindGameObjectWithTag("PortalArea").GetComponent<BoxCollider2D>();
        PlaceInitialPortals();
    }

    void Update()
    {
        if (!PowerUpSelection.IsUIActive) // Only handle input if UI is not active
        {
            HandlePortalPlacement();
        }

    }

    private void PlaceInitialPortals()
    {
        bluePortal = Instantiate(bluePortalPrefab, new Vector3(-5, 0, 0), Quaternion.identity);
        redPortal = Instantiate(redPortalPrefab, new Vector3(5, 0, 0), Quaternion.identity);
        bluePortal.GetComponent<Portal>().SetPortalController(this, "blue");
        redPortal.GetComponent<Portal>().SetPortalController(this, "red");
    }

    private void HandlePortalPlacement()
    {
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        if (Input.GetMouseButtonDown(0) && IsWithinPlacementArea(mousePosition))
        {
            SetPortalPosition(bluePortal, mousePosition);
        }
        if (Input.GetMouseButton(0) && bluePortal != null)
        {
            UpdatePortalOrientation(bluePortal);
        }
        if (Input.GetMouseButtonDown(1) && IsWithinPlacementArea(mousePosition))
        {
            SetPortalPosition(redPortal, mousePosition);
        }
        if (Input.GetMouseButton(1) && redPortal != null)
        {
            UpdatePortalOrientation(redPortal);
        }
    }

    private void SetPortalPosition(GameObject portal, Vector3 position)
    {
        portal.transform.position = position;
    }

    private void UpdatePortalOrientation(GameObject portal)
{
    Vector2 direction = mousePosition - portal.transform.position;
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    portal.transform.rotation = Quaternion.Euler(0, 0, angle);
}
    private bool IsWithinPlacementArea(Vector3 position)
    {
        return allowedPlacementArea.OverlapPoint(position);
    }

    public void EnterPortal(GameObject obj, string portalColor)
{
    if (obj.tag == "Boundary") return; // Skip boundaries

    if (obj.tag == "Portal") return; // Skip portals

    if (obj.tag == "LaserEvent") return; // Skip laser events

    // Check if the object is on cooldown to avoid immediate re-entry problems
    if (teleportCooldowns.ContainsKey(obj) && teleportCooldowns[obj] > Time.time)
    {
        
        return;
    }

    GameObject exitPortal = portalColor == "blue" && redPortal ? redPortal : bluePortal;

        if (exitPortal)
        {
            // Randomly decide based on blender odds whether to destroy or teleport
            if (Random.value < blenderOdds / 100.0f && obj.tag == "Enemy")
            {
                
                obj.GetComponent<EnemyHealth>().Die();
                
            }
            else
            {
                Teleport(obj, exitPortal.transform.position, exitPortal.transform.rotation);
            }
            teleportCooldowns[obj] = Time.time + teleportCooldownDuration; // update cooldown
        }
    }
   
public void IncreaseBlender() {
    blenderOdds += 20f;
    
}

   private void Teleport(GameObject obj, Vector3 position, Quaternion exitRotation) 
{
    Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        // Assuming the exit direction is aligned with the portal's right vector
        Vector2 exitDirection = exitRotation * Vector2.right;

        // Ensure there is a minimum speed
        float minSpeed = 2f; // Define a suitable minimum speed
        float currentSpeed = rb.velocity.magnitude;
        
        // If the current speed is less than the minimum speed, set to minimum speed
        if (currentSpeed < minSpeed) {
            currentSpeed = minSpeed;
        }

        // Set the new velocity in the direction the exit portal is facing
        rb.velocity = exitDirection.normalized * currentSpeed;

        // Update the rotation to face along the direction of velocity
        // Calculate the angle from the velocity vector
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        
        // Assuming the Drop Pod's forward direction is 'up' in sprite, subtract 90 degrees
        obj.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        // Update the position to the exit portal's position
        obj.transform.position = position;
    }
}
}