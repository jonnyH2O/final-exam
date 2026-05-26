using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject container; // Reference to the pause menu container
    private float levelStartTime;

    private void Start()
    {
        levelStartTime = Time.time;
    }

    void Update()
    {
        // Disable pause menu for the first 3 seconds
        if (Time.time - levelStartTime < 3f)
            return;

        // Don't allow pause/resume if tutorial is active
        if (TutorialManager.Instance != null && TutorialManager.Instance.TutorialActive)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameManager.IsPaused)
            {
                ResumeButton();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        container.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Pause the game
        GameManager.IsPaused = true;
    }

    public void ResumeButton()
    {
        container.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume the game
        GameManager.IsPaused = false;
    }

    public void MainMenuButton()
    {
        // Ensure the game is unpaused before switching scenes
        Time.timeScale = 1f;
        GameManager.IsPaused = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}

