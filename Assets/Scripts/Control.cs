using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Control : MonoBehaviour
{
    public GameObject[] Background;
    private int index;

    public void Home() {
        LevelMenu.ResetUnlocked();
        SceneManager.LoadScene("LevelManager");
    }

    void Start()
    {
        index = 0;
        UpdateBackgroundDisplay();
    }

    void UpdateBackgroundDisplay()
    {
        for (int i = 0; i <= 4; i++)
        {
            Background[i].gameObject.SetActive(false);
            Debug.Log("Background " + i + (i == index ? " is Active" : " is Inactive"));
        }
        Background[index].gameObject.SetActive(true);
        
    }

    public void Next()
    {
        if (index < 4)
        {
            index++;
            UpdateBackgroundDisplay();
        }
        Debug.Log(index);
    }

    public void Previous()
    {
        if (index > 0)
        {
            index--;
            UpdateBackgroundDisplay();
        }
        Debug.Log(index);
    }

    
}
