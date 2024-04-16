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

    private Queue<GameObject> blueStoredObjects = new Queue<GameObject>();
    private Queue<GameObject> redStoredObjects = new Queue<GameObject>();
    private Dictionary<GameObject, float> ignorePortalTime = new Dictionary<GameObject, float>();
    private float ignoreDuration = 0.5f;  // Time in seconds to ignore portal after ejecting

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandlePortalPlacement();
        ClearIgnoreList();
    }

    private void HandlePortalPlacement()
    {
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the z-position is zero to keep it in 2D plane

        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            if (bluePortal != null) // If already placed, pick it up
                Destroy(bluePortal);
            else
            {
                bluePortal = Instantiate(bluePortalPrefab, mousePosition, Quaternion.identity);
                bluePortal.GetComponent<Portal>().SetPortalController(this, "blue");
            }
        }
        if (Input.GetMouseButton(0) && bluePortal != null) // Adjust direction while holding
        {
            Vector3 direction = mousePosition - bluePortal.transform.position;
            bluePortal.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            if (redPortal != null) // If already placed, pick it up
                Destroy(redPortal);
            else
            {
                redPortal = Instantiate(redPortalPrefab, mousePosition, Quaternion.identity);
                redPortal.GetComponent<Portal>().SetPortalController(this, "red");
            }
        }
        if (Input.GetMouseButton(1) && redPortal != null) // Adjust direction while holding
        {
            Vector3 direction = mousePosition - redPortal.transform.position;
            redPortal.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }

    public void EnterPortal(GameObject obj, string portalColor)
    {
        if (ignorePortalTime.ContainsKey(obj) && Time.time < ignorePortalTime[obj])
            return;  // Ignore this object if it's in the cooldown period

        if (portalColor == "blue" && redPortal != null)
        {
            Eject(obj, redPortal.transform.position, redPortal.transform.up, "red");
        }
        else if (portalColor == "red" && bluePortal != null)
        {
            Eject(obj, bluePortal.transform.position, bluePortal.transform.up, "blue");
        }
        else
        {
            StoreObject(obj, portalColor);
        }
    }

    private void StoreObject(GameObject obj, string portalColor)
    {
        obj.SetActive(false);
        if (portalColor == "blue")
            blueStoredObjects.Enqueue(obj);
        else
            redStoredObjects.Enqueue(obj);
    }

    private void Eject(GameObject obj, Vector3 position, Vector3 direction, string portalColor)
    {
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.up = direction;
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * rb.velocity.magnitude; // Maintain the speed but update the direction
        }
        SetIgnore(obj, portalColor);
    }

    private void SetIgnore(GameObject obj, string portalColor)
    {
        if (!ignorePortalTime.ContainsKey(obj))
            ignorePortalTime[obj] = Time.time + ignoreDuration;
        else
            ignorePortalTime[obj] = Time.time + ignoreDuration;
    }

    private void ClearIgnoreList()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var pair in ignorePortalTime)
        {
            if (Time.time > pair.Value)
                toRemove.Add(pair.Key);
        }

        foreach (var obj in toRemove)
            ignorePortalTime.Remove(obj);
    }

    private void ReleaseStoredObjects()
    {
        while (blueStoredObjects.Count > 0)
        {
            GameObject obj = blueStoredObjects.Dequeue();
            Eject(obj, redPortal.transform.position, redPortal.transform.up, "red");
        }
        while (redStoredObjects.Count > 0)
        {
            GameObject obj = redStoredObjects.Dequeue();
            Eject(obj, bluePortal.transform.position, bluePortal.transform.up, "blue");
        }
    }
}