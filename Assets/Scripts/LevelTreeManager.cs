using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTreeManager : MonoBehaviour
{
    public List<LevelNode> levelNodes; // List of all nodes in the tree

    void Start()
    {
        InitializeTree();
    }

    void InitializeTree()
    {
        // Unlock the first node (level 1)
        levelNodes[0].isUnlocked = true;

        // Here, you can define logic to unlock nodes based on player's progress
        for (int i = 1; i < levelNodes.Count; i++)
        {
            // Example logic: if previous node is completed, unlock the next node
            if (PlayerPrefs.GetInt("Level" + (i), 0) == 1)
            {
                levelNodes[i].isUnlocked = true;
            }

            levelNodes[i].UpdateNodeState();
        }
    }

    public void CompleteLevel(int levelIndex)
    {
        // Mark the current level as completed and unlock the next node
        PlayerPrefs.SetInt("Level" + levelIndex, 1);

        if (levelIndex + 1 < levelNodes.Count)
        {
            levelNodes[levelIndex + 1].isUnlocked = true;
            levelNodes[levelIndex + 1].UpdateNodeState();
        }
    }
}
