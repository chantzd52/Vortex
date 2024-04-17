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

    private Queue<GameObject> queueForRed = new Queue<GameObject>();
    private Dictionary<GameObject, Vector2> originalVelocities = new Dictionary<GameObject, Vector2>();
    private Dictionary<GameObject, float> cooldownTimes = new Dictionary<GameObject, float>();

     public CircleCollider2D allowedPlacementArea; // Reference to the CircleCollider2D that defines where portals can be placed

    public float cooldownDuration = 2.0f; // Cooldown to prevent immediate re-entry
    public float ejectDelay = 1.0f; // Delay between ejecting objects from the red portal

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandlePortalPlacement();
        TryReactivateObjects(redPortal, queueForRed);
    }

    private void HandlePortalPlacement()
{
    mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    mousePosition.z = 0;  // Adjust for 2D gameplay

    if (Input.GetMouseButtonDown(0) && IsWithinPlacementArea(mousePosition)) // Left mouse button for blue portal
    {
        TogglePortal(ref bluePortal, bluePortalPrefab, "blue");
    }
    if (Input.GetMouseButton(0) && bluePortal != null) // Left mouse button held down
    {
        // Update blue portal's orientation to face the mouse
        Vector2 direction = mousePosition - bluePortal.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bluePortal.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    if (Input.GetMouseButtonDown(1) && IsWithinPlacementArea(mousePosition)) // Right mouse button for red portal
    {
        TogglePortal(ref redPortal, redPortalPrefab, "red");
    }
    if (Input.GetMouseButton(1) && redPortal != null) // Right mouse button held down
    {
        // Update red portal's orientation to face the mouse
        Vector2 direction = mousePosition - redPortal.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        redPortal.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}

private bool IsWithinPlacementArea(Vector3 position)
{
    return allowedPlacementArea.OverlapPoint(position);  // Check if the position is within the allowed area
}

    private void TogglePortal(ref GameObject portal, GameObject portalPrefab, string color)
    {
        if (portal == null)
        {
            portal = Instantiate(portalPrefab, mousePosition, Quaternion.identity);
            portal.GetComponent<Portal>().SetPortalController(this, color);
        }
        else
        {
            portal.SetActive(!portal.activeSelf);
            if (portal.activeSelf)
                portal.transform.position = mousePosition;
        }
    }


    public void EnterPortal(GameObject obj, string portalColor)
    {
        if (portalColor == "blue" && obj.tag == "Player")
        {
            if (redPortal != null && redPortal.activeSelf)
            {
                TeleportPlayer(obj, redPortal.transform.position);
            }
            else
            {
                GameOver(); // Red portal not active, game over
            }
        }
        else if (portalColor == "blue" && obj.tag != "Player")
        {
            if (cooldownTimes.ContainsKey(obj) && Time.time < cooldownTimes[obj]) return;

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                originalVelocities[obj] = rb.velocity; // Store original velocity
            }

            SetObjectInactive(obj);
            queueForRed.Enqueue(obj);
            cooldownTimes[obj] = Time.time + cooldownDuration;
        }
    }

    private void TeleportPlayer(GameObject player, Vector3 exitPosition)
    {
        player.transform.position = exitPosition; // Teleport player to red portal position
    }

    private void GameOver()
    {
        Debug.Log("Game Over: Red portal not active!");
        // Implement actual game over mechanics (UI display, scene reset, etc.)
    }

    private void TryReactivateObjects(GameObject portal, Queue<GameObject> queue)
    {
        if (portal != null && portal.activeSelf && queue.Count > 0)
        {
            GameObject obj = queue.Dequeue();
            ReactivateObject(obj, portal.transform.position);
        }
    }

    private void ReactivateObject(GameObject obj, Vector3 position)
{
    obj.SetActive(true);
    obj.transform.position = position;
    Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
    if (rb != null && originalVelocities.ContainsKey(obj))
    {
        rb.isKinematic = false;

        // Apply the stored velocity magnitude in the direction the portal is facing
        Vector2 exitDirection = redPortal.transform.right; // Assuming the portal's "forward" direction is along its local X-axis
        rb.velocity = exitDirection * originalVelocities[obj].magnitude;
        
        originalVelocities.Remove(obj);
    }
}

    private void SetObjectInactive(GameObject obj)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        obj.SetActive(false);
    }
}