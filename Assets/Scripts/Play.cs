using UnityEngine.SceneManagement;
using UnityEngine;

public class Play : MonoBehaviour
{
    public void onClick() {
        SceneManager.LoadScene("Gameplay");
    }
}
