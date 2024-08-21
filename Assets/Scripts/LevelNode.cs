using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelNode : MonoBehaviour
{
    public string levelName; // The name of the scene to load
    public bool isUnlocked = true; // Whether the node is unlocked
    private Button button; // Reference to the button component

    void Start()
    {
        // Find the Button component in the child objects
        button = GetComponentInChildren<Button>();

    }

    public void UpdateNodeState()
    {
        // Ensure the button is not null before accessing it
        if (button != null)
        {
            button.interactable = isUnlocked; // Make the button interactable only if the node is unlocked
        }
    }

    public void OnClick()
    {
        if (isUnlocked)
        {
            Debug.Log("Loading level: " + levelName);
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.Log("Node is locked: " + gameObject.name);
        }
    }
}
