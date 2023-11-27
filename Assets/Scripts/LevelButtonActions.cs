using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class LevelButtonActions : MonoBehaviour
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

    //Pause level
    public void pause(){
        //Pause
        if(!Gameplay.level.paused){
            //Disable game action
            Gameplay.level.paused = true;
            Gameplay.level.enabled = false;
            Gameplay.level.allowSwaps = false;
            //Show text and buttons
            Gameplay.level.pauseText.SetActive(true);
            Gameplay.level.levelEndRetry.SetActive(true);
            Gameplay.level.levelEndQuit.SetActive(true);
            Gameplay.level.resumeButton.SetActive(true);
        }
        //Unpause
        else{
            //Enable game action
            Gameplay.level.paused = false;
            Gameplay.level.enabled = true;
            Gameplay.level.allowSwaps = true;
            //Hide text and buttons
            Gameplay.level.pauseText.SetActive(false);
            Gameplay.level.levelEndRetry.SetActive(false);
            Gameplay.level.levelEndQuit.SetActive(false);
            Gameplay.level.resumeButton.SetActive(false);
        }
    }

    //Unpause
    public void resume(){
        //Enable game action
            Gameplay.level.paused = false;
            Gameplay.level.enabled = true;
            Gameplay.level.allowSwaps = true;
            //Hide text and buttons
            Gameplay.level.pauseText.SetActive(false);
            Gameplay.level.levelEndRetry.SetActive(false);
            Gameplay.level.levelEndQuit.SetActive(false);
            Gameplay.level.resumeButton.SetActive(false);
    }

}
