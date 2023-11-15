using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] lvlButtons;
    public static void ResetUnlocked() 
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save(); 
    }

    private void Unlock() 
    {
        int unlockedlevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < lvlButtons.Length; i++)
        {
            lvlButtons[i].interactable = false;
            //Debug.Log("Locking level button index: " + i); 
        }

        for (int i = 0; i < unlockedlevel; i++)
        {
            if (i < lvlButtons.Length) 
            {
                lvlButtons[i].interactable = true;
                //Debug.Log("Unlocking level button index: " + i); 
            }
        }
    }

    private void Start() 
    {
        //ResetUnlocked();
        Unlock();
    }

    public void OpenLevel(int a){
        PlayerPrefs.SetInt("SelectedLevel", a);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Gameplay");
    }
}
