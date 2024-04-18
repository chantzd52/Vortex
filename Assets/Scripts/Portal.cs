using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private PortalController portalController;
    private string portalColor;

    public void SetPortalController(PortalController controller, string color)
    {
        portalController = controller;
        portalColor = color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (portalColor == "blue")
        {
            portalController.EnterPortal(other.gameObject, portalColor);
        }

        if (portalColor == "red")
        {
            portalController.EnterPortal(other.gameObject, portalColor);
        }
    }
}