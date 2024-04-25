using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PowerUpSelection : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpDetails
    {
        public string name;
        public Sprite image;
        public string description;
        public PowerUpType type;
        public int selectionCount = 0;  // Track the number of times selected
        public int maxSelections = 3;   // Maximum number of times it can be selected
    }

    public enum PowerUpType
    {
        GainShieldSlot,  // Increases max bullet shields
        IncreaseMaxLaserShields,  // Increases max laser shields
        IncreaseSpeed,  // Increases player speed
        IncreaseTurnSpeed,  // Increases player turn speed
        AddBulletShield, // Adds a bullet shield
        AddLaserShield, // Adds a laser shield
        Portalsize, // Increases the size of the portal
        ShipSize, // Increases the size of the ship
        Blender, // Blends the ship
    }

    [System.Serializable]
    public class PowerUpSlot
{
    public Button button; // The button that also displays the power up image
    public TextMeshProUGUI descriptionText; // Text for the power up's description
}

    public GameObject powerUpUI;
    public PowerUpSlot[] powerUpSlots;
    public static bool IsUIActive = false;
    public List<PowerUpDetails> availablePowerUps;
    public SpaceshipController SpaceshipController;

    void Start()
    {
        HidePowerUpSelection();
    }

    public void ShowPowerUpSelection()
    {
        powerUpUI.SetActive(true);
        IsUIActive = true;
        Time.timeScale = 0;
        SetupRandomPowerUps();
    }

   private void SetupRandomPowerUps()
{
    List<PowerUpDetails> filterAvailablePowerUps = availablePowerUps.FindAll(p =>
        (p.selectionCount < p.maxSelections) ||
        p.type == PowerUpType.AddBulletShield ||
        p.type == PowerUpType.AddLaserShield);

    List<PowerUpDetails> chosenPowerUps = new List<PowerUpDetails>();

    while (chosenPowerUps.Count < Mathf.Min(powerUpSlots.Length, filterAvailablePowerUps.Count))
    {
        int randomIndex = Random.Range(0, filterAvailablePowerUps.Count);
        if (!chosenPowerUps.Contains(filterAvailablePowerUps[randomIndex]))
            chosenPowerUps.Add(filterAvailablePowerUps[randomIndex]);
    }

    for (int i = 0; i < powerUpSlots.Length; i++)
    {
        if (i < chosenPowerUps.Count)
        {
            PowerUpDetails powerUp = chosenPowerUps[i];
            PowerUpSlot slot = powerUpSlots[i];

            slot.descriptionText.text = powerUp.description;
            Image buttonImage = slot.button.GetComponent<Image>();
            buttonImage.sprite = powerUp.image;
            slot.button.onClick.RemoveAllListeners();
            slot.button.onClick.AddListener(() => ApplyPowerUp(powerUp));
            slot.button.interactable = true;
        }
        else
        {
            powerUpSlots[i].button.gameObject.SetActive(false);
        }
    }
}



    public void HidePowerUpSelection()
    {
        powerUpUI.SetActive(false);
        IsUIActive = false;
        Time.timeScale = 1;
    }

    private void UpdatePowerUpAvailability()
{
    // Re-filter and update UI based on new counts
    SetupRandomPowerUps();
}

    private void ApplyPowerUp(PowerUpDetails powerUp)
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            switch (powerUp.type)
            {
                case PowerUpType.GainShieldSlot:
                    player.AddBulletShieldSlot();
                    break;
                case PowerUpType.IncreaseMaxLaserShields:
                    player.AddLaserShieldSlot();
                    break;
                case PowerUpType.IncreaseSpeed:
                    player.IncreaseThrustPower();  
                    break;
                case PowerUpType.IncreaseTurnSpeed:
                    player.IncreaseTurnSpeed();  
                    break;
                case PowerUpType.AddBulletShield:
                    player.AddBulletShield();
                    break;
                case PowerUpType.AddLaserShield:
                    player.AddLaserShield();
                    break;
                case PowerUpType.Portalsize:
                    player.IncreasePortalSize();
                    break;
                case PowerUpType.ShipSize:
                    player.DecreaseShipSize();
                    break;
            }
            powerUp.selectionCount++;
            UpdatePowerUpAvailability();
            HidePowerUpSelection();
            player.BecomeInvincible(); 
        }
        else
        {
            Debug.LogError("Player object not found.");
        }
    }
}