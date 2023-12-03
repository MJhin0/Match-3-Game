using UnityEngine.SceneManagement;
using UnityEngine;

public class Play : MonoBehaviour
{
    public void PlayNow() {
        LevelMenu.ResetUnlocked();
        SceneManager.LoadScene("LevelManager");
    }

    public void Tutorial() {
        LevelMenu.ResetUnlocked();
        SceneManager.LoadScene("Tutorial");
    }

    public void ExitNow() {
        Application.Quit();
    }
}
