using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(string sceneName)
    {
        // Temporarily hardcoded
        SceneManager.LoadScene(sceneName);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


}
