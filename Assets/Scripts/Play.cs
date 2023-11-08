using UnityEngine.SceneManagement;
using UnityEngine;

public class Play : MonoBehaviour
{
    public void onClick() {
        LevelMenu.ResetUnlocked();
        SceneManager.LoadScene("LevelManager");
    }
}
