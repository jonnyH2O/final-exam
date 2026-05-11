using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompleteScreen;
    [SerializeField] private GameObject fullClearText;

    // CHANGED: how long the level complete screen stays up before loading the next scene
    [SerializeField] private float nextLevelDelay = 3f;

    private void Awake()
    {

        Instance = this;

        gameOverScreen.SetActive(false);
        levelCompleteScreen.SetActive(false);
        fullClearText.SetActive(false);
    }

    public void ShowGameOver()
    {
        GameManager.IsPaused = true;

        gameOverScreen.SetActive(true);
    }

    public void ShowLevelComplete()
    {
        GameManager.IsPaused = true;

        levelCompleteScreen.SetActive(true);

        if (ScoreManager.Instance != null &&
            ScoreManager.Instance.FullClear)
        {
            fullClearText.SetActive(true);
        }
        else
        {
            fullClearText.SetActive(false);
        }

        // CHANGED: queue the next level load after a short delay so the player can see the screen
        StartCoroutine(LoadNextLevelAfterDelay());
    }

    // CHANGED: waits in realtime (immune to Time.timeScale) then loads the next scene by build index
    private IEnumerator LoadNextLevelAfterDelay()
    {
        yield return new WaitForSecondsRealtime(nextLevelDelay);

        GameManager.IsPaused = false;

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            // No more levels — fall back to the main menu (build index 0)
            SceneManager.LoadScene(0);
        }
    }
}