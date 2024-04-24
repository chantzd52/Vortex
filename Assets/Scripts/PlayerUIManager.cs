using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public Image[] bulletShieldIcons; // UI icons for bullet shields
    public Image[] laserShieldIcons; // UI icons for laser shields

    // Method to update bullet shield icons
    public void UpdateBulletShieldIcons(int currentShields, int maxSlots)
    {
        for (int i = 0; i < bulletShieldIcons.Length; i++)
        {
            bulletShieldIcons[i].gameObject.SetActive(i < maxSlots);
            bulletShieldIcons[i].color = i < currentShields ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.1f); // Full opacity for filled slots, semi-transparent for empty slots
        }
    }

    // Method to update laser shield icons
    public void UpdateLaserShieldIcons(int currentShields, int maxSlots)
    {
        for (int i = 0; i < laserShieldIcons.Length; i++)
        {
            laserShieldIcons[i].gameObject.SetActive(i < maxSlots);
            laserShieldIcons[i].color = i < currentShields ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.3f); // Full opacity for filled slots, semi-transparent for empty slots
        }
    }
}
