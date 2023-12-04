using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Control : MonoBehaviour
{

    public void Home() {
        LevelMenu.ResetUnlocked();
        SceneManager.LoadScene("LevelManager");
    }

    void Start()
    {
    }

}
