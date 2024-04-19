using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PowerUpSelection : MonoBehaviour
{
    public GameObject powerUpUI; // Assign this in the inspector, should be your UI panel
    public Button[] powerUpButtons; // Assign buttons for power-ups
    public static bool IsUIActive = false; // Static variable to track UI state

    private List<string> availablePowerUps = new List<string>() { "Shield", "Shield", "Shield" }; // List of all power-ups

    void Start()
    {
        HidePowerUpSelection();
        ConfigureButtons(); // Configure buttons without assigning specific actions
    }

    public void ShowPowerUpSelection()
    {
        powerUpUI.SetActive(true);
        IsUIActive = true; // Set the UI as active
        Time.timeScale = 0; // Pause the game
        SetupRandomPowerUps();
    }

    private void SetupRandomPowerUps()
{
    List<string> chosenPowerUps = new List<string>();

    if (powerUpButtons == null)
    {
        Debug.LogError("PowerUpButtons array is not assigned!");
        return;
    }

    for (int i = 0; i < powerUpButtons.Length; i++)
    {
        if (powerUpButtons[i] == null)
        {
            Debug.LogError($"PowerUp button at index {i} is not assigned!");
            continue;
        }

        // Get the TextMeshPro component from the button's children
        TextMeshProUGUI buttonText = powerUpButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText == null)
        {
            Debug.LogError($"No TextMeshProUGUI component found for PowerUp button at index {i}!");
            continue;
        }

        int randIndex = Random.Range(0, availablePowerUps.Count);
        string powerUpType = availablePowerUps[randIndex];
        chosenPowerUps.Add(powerUpType);

        buttonText.text = powerUpType;  // Set the button text using TextMeshPro
    }

    if (chosenPowerUps.Count > 0)
    {
        ConfigureButtons(chosenPowerUps);
    }
}

    public void HidePowerUpSelection()
    {
        powerUpUI.SetActive(false);
        IsUIActive = false; // Set the UI as inactive
        Time.timeScale = 1; // Resume the game
    }

    // Setup buttons without specific actions
   private void ConfigureButtons()
{
    if (powerUpButtons == null || powerUpButtons.Length == 0)
    {
        Debug.LogError("PowerUp buttons are not assigned in the inspector");
        return;
    }

    foreach (Button button in powerUpButtons)
    {
        if (button != null)
        {
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = "Select Power-Up";
            }
            button.onClick.RemoveAllListeners();
            button.interactable = false;  // Disable until options are available
        }
        else
        {
            Debug.LogError("One of the buttons in the array is not assigned!");
        }
    }
}

    // Configure buttons with dynamically assigned power-ups
    private void ConfigureButtons(List<string> chosenPowerUps)
{
    for (int i = 0; i < powerUpButtons.Length; i++)
    {
        if (i < chosenPowerUps.Count)  // Check if there's a power-up assigned for this button
        {
            int index = i;  // Local copy for lambda capture
            powerUpButtons[i].interactable = true;  // Enable the button
            powerUpButtons[i].onClick.RemoveAllListeners();
            powerUpButtons[i].onClick.AddListener(() => SelectPowerUp(chosenPowerUps[index]));
        }
    }
}

    void SelectPowerUp(string powerUpType)
{
    Debug.Log($"Power Up Selected: {powerUpType}");
    Player player = FindObjectOfType<Player>(); // Find the player object
    if (player != null)
    {
        if (powerUpType == "Shield")
        {
            player.bulletShield++; // Increase shield
            Debug.Log("Shield increased");
        }
        // Trigger invincibility regardless of the power-up type
        player.BecomeInvincible();

        HidePowerUpSelection(); // Hide UI and resume the game
    }
    else
    {
        Debug.LogError("Player object not found.");
    }
}
}
