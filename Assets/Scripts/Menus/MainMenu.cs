using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(string sceneName)
    {
        LevelAudioManager.Instance?.FadeOutAll();
        if (CinematicTransition.Instance != null)
        {
            CinematicTransition.Instance.LoadSceneWithTransition(
                sceneName,
                "THE TRIAL BEGINS"
            );
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void LoadLevel(string sceneName)
    {
        LevelAudioManager.Instance?.FadeOutAll();
        if (CinematicTransition.Instance != null)
        {
            CinematicTransition.Instance.LoadSceneWithTransition(
                sceneName,
                "A NEW BATTLE BEGINS"
            );
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}