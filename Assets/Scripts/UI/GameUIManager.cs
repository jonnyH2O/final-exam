using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompleteScreen;
    [SerializeField] private GameObject fullClearText;

    [SerializeField] private TMP_Text retryText;
    [SerializeField] private TMP_Text continueText;

    private bool waitingForContinue;
    private bool waitingForRetry;

    private void Awake()
    {
        Instance = this;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        if (levelCompleteScreen != null)
            levelCompleteScreen.SetActive(false);

        if (fullClearText != null)
            fullClearText.SetActive(false);

        if (retryText != null)
        {
            retryText.text =
                "PRESS [SPACE] TO RETRY";
        }

        if (continueText != null)
        {
            continueText.text =
                "PRESS [SPACE] TO CONTINUE";
        }

    }

    private void Update()
    {
        bool spacePressed =
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame;

        if (waitingForRetry && spacePressed)
        {
            waitingForRetry = false;

            StartCoroutine(RestartCurrentLevel());
        }

        if (waitingForContinue && spacePressed)
        {
            waitingForContinue = false;

            StartCoroutine(LoadNextLevel());
        }
    }

    public void ShowGameOver()
    {
        GameManager.IsPaused = true;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        if (levelCompleteScreen != null)
            levelCompleteScreen.SetActive(false);

        if (fullClearText != null)
            fullClearText.SetActive(false);

        waitingForRetry = true;
        waitingForContinue = false;
    }

    public void ShowLevelComplete()
    {
        GameManager.IsPaused = true;

        if (levelCompleteScreen != null)
            levelCompleteScreen.SetActive(true);

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        if (fullClearText != null)
        {
            bool fullClear =
                ScoreManager.Instance != null &&
                ScoreManager.Instance.FullClear;

            fullClearText.SetActive(fullClear);
        }

        waitingForContinue = true;
        waitingForRetry = false;
    }

    private IEnumerator RestartCurrentLevel()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        if (levelCompleteScreen != null)
            levelCompleteScreen.SetActive(false);

        if (fullClearText != null)
            fullClearText.SetActive(false);

        yield return new WaitForSecondsRealtime(0.1f);

        GameManager.IsPaused = false;
        Time.timeScale = 1f;
        string currentScene =
            SceneManager.GetActiveScene().name;

        if (CinematicTransition.Instance != null)
        {
            CinematicTransition.Instance.LoadSceneWithTransition(
                currentScene,
                "TRY AGAIN..."
            );
        }
        else
        {
            SceneManager.LoadScene(currentScene);
        }
    }

    private IEnumerator LoadNextLevel()
    {
        if (levelCompleteScreen != null)
            levelCompleteScreen.SetActive(false);

        if (fullClearText != null)
            fullClearText.SetActive(false);

        yield return new WaitForSecondsRealtime(0.1f);

        GameManager.IsPaused = false;

        int nextIndex =
            SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            string nextScene =
                SceneUtility.GetScenePathByBuildIndex(nextIndex)
                .Split('/')[^1]
                .Replace(".unity", "");

            if (CinematicTransition.Instance != null)
            {
                CinematicTransition.Instance.LoadSceneWithTransition(
                    nextScene,
                    "THE TEST CONTINUES IN A NEW DOMAIN..."
                );
            }
            else
            {
                SceneManager.LoadScene(nextScene);
            }
        }
        else
        {
            if (CinematicTransition.Instance != null)
            {
                CinematicTransition.Instance.LoadSceneWithTransition(
                    "MainMenu",
                    "PEACE RETURNS..."
                );
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}