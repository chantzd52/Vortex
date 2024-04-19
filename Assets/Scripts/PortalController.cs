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
    public float teleportCooldownDuration = 0.5f; // seconds
    public BoxCollider2D allowedPlacementArea; 

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

    // Check if the object is on cooldown to avoid immediate re-entry problems
    if (teleportCooldowns.ContainsKey(obj) && teleportCooldowns[obj] > Time.time)
    {
        
        return;
    }

    GameObject exitPortal = null;
    Quaternion exitDirection = Quaternion.identity;

    if (portalColor == "blue" && redPortal != null && redPortal.activeSelf)
    {
        exitPortal = redPortal;
        exitDirection = redPortal.transform.rotation;
    }
    else if (portalColor == "red" && bluePortal != null && bluePortal.activeSelf)
    {
        exitPortal = bluePortal;
        exitDirection = bluePortal.transform.rotation;
    }

    if (exitPortal != null)
    {
        // Immediately teleport without deactivating
        Teleport(obj, exitPortal.transform.position, exitDirection);
        teleportCooldowns[obj] = Time.time + teleportCooldownDuration;  // update cooldown
    }
    else
    {
        Debug.Log("Game Over: Corresponding portal not active!");
    }
}

   private void Teleport(GameObject obj, Vector3 position, Quaternion exitRotation)
{
    Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        Vector2 exitDirection = exitRotation * Vector3.right;  // Assuming portal 'right' is the forward direction
        rb.velocity = exitDirection.normalized * rb.velocity.magnitude;  // Maintain speed, change direction
        obj.transform.position = position;
    }
}
}