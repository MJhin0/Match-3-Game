using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelActions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Reloads the level, on win OR loss
    public void retry(){
        SceneManager.LoadScene("Gameplay");
    }

    //Goes back to level select, no unlock
    public void quit(){
        SceneManager.LoadScene("LevelManager");
    }

    //Goes back to level select, unlock if applicable
    public void levelUnlock(){
        int currentLevelIndex = PlayerPrefs.GetInt("SelectedLevel", 1);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Only unlock the next level if it's not already unlocked
        if (currentLevelIndex >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevelIndex + 1);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene("LevelManager");
    }
}
