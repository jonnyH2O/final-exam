using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompleteScreen;
    [SerializeField] private GameObject fullClearText;

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
    }
}