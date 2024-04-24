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
    }

    public enum PowerUpType
    {
        GainShieldSlot,  // Increases max bullet shields
        IncreaseMaxLaserShields,  // Increases max laser shields
        IncreaseSpeed,  // Increases player speed
        IncreaseTurnSpeed,  // Increases player turn speed
        AddBulletShield  // Adds a bullet shield

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
    List<PowerUpDetails> chosenPowerUps = new List<PowerUpDetails>();

    while (chosenPowerUps.Count < Mathf.Min(powerUpSlots.Length, availablePowerUps.Count))
    {
        int randomIndex = Random.Range(0, availablePowerUps.Count);
        if (!chosenPowerUps.Contains(availablePowerUps[randomIndex]))
        {
            chosenPowerUps.Add(availablePowerUps[randomIndex]);
        }
    }

    for (int i = 0; i < powerUpSlots.Length; i++)
    {
        if (i < chosenPowerUps.Count)
        {
            PowerUpDetails powerUp = chosenPowerUps[i];
            PowerUpSlot slot = powerUpSlots[i];

            slot.descriptionText.text = powerUp.description;
            Image buttonImage = slot.button.GetComponent<Image>(); // Get the Image component of the button
            buttonImage.sprite = powerUp.image; // Set the sprite to the power up image
            slot.button.onClick.RemoveAllListeners();
            slot.button.onClick.AddListener(() => ApplyPowerUp(powerUp.type));
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

    private void ApplyPowerUp(PowerUpType type)
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            switch (type)
            {
                case PowerUpType.GainShieldSlot:
                    player.AddBulletShieldSlot();
                    break;
                case PowerUpType.IncreaseMaxLaserShields:
                    player.AddLaserShieldSlot();
                    break;
                case PowerUpType.IncreaseSpeed:
                    player.IncreaseSpeed();  
                    break;
                case PowerUpType.IncreaseTurnSpeed:
                    player.IncreaseTurnSpeed();  
                    break;
                case PowerUpType.AddBulletShield:
                    player.AddBulletShield();
                    break;
            }
            HidePowerUpSelection();
            player.BecomeInvincible();  // Make the player invincible after selecting a power up
        }
        else
        {
            Debug.LogError("Player object not found.");
        }
    }
}