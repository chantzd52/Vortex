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
public float teleportCooldownDuration = 1.0f; // seconds
    public CircleCollider2D allowedPlacementArea; // Area where portals can be placed

    void Start()
    {
        mainCamera = Camera.main;
        PlaceInitialPortals();
    }

    void Update()
    {
        HandlePortalPlacement();
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
        portal.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private bool IsWithinPlacementArea(Vector3 position)
    {
        return allowedPlacementArea.OverlapPoint(position);
    }

    public void EnterPortal(GameObject obj, string portalColor)
{
    if (obj.tag == "Boundary") return; // Skip boundaries

    // Check if the object is on cooldown
    if (teleportCooldowns.ContainsKey(obj) && teleportCooldowns[obj] > Time.time)
    {
        Debug.Log($"{obj.name} is on cooldown.");
        return;
    }

    GameObject exitPortal = null;
    Vector3 exitDirection = Vector3.right;

    if (portalColor == "blue" && redPortal != null && redPortal.activeSelf)
    {
        exitPortal = redPortal;
        exitDirection = redPortal.transform.right;
    }
    else if (portalColor == "red" && bluePortal != null && bluePortal.activeSelf)
    {
        exitPortal = bluePortal;
        exitDirection = bluePortal.transform.right;
    }

    if (exitPortal != null)
    {
        Teleport(obj, exitPortal.transform.position, exitDirection);
        // Update cooldown
        teleportCooldowns[obj] = Time.time + teleportCooldownDuration;
    }
    else
    {
        Debug.Log("Game Over: Corresponding portal not active!");
    }
}

    private void Teleport(GameObject obj, Vector3 position, Vector3 exitDirection)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null && originalVelocities.ContainsKey(obj))
        {
            rb.velocity = exitDirection * originalVelocities[obj].magnitude;
            originalVelocities.Remove(obj);
        }
        obj.transform.position = position;
        obj.SetActive(true);
    }
}