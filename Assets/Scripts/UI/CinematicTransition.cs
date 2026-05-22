using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CinematicTransition : MonoBehaviour
{
    public static CinematicTransition Instance;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Cinematic Text")]
    [SerializeField] private TMP_Text cinematicText;
    [SerializeField] private float textDuration = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1f;
            fadeCanvas.blocksRaycasts = true;
        }

        if (cinematicText != null)
        {
            cinematicText.text = "";
        }

        StartCoroutine(FadeIn());
    }

    public void LoadSceneWithTransition(string sceneName, string introText = "")
    {
        StartCoroutine(Transition(sceneName, introText));
    }

    private IEnumerator Transition(string sceneName, string introText)
    {
        yield return StartCoroutine(FadeOut());

        if (cinematicText != null)
        {
            cinematicText.text = introText;
        }

        if (!string.IsNullOrEmpty(introText))
        {
            yield return new WaitForSecondsRealtime(textDuration);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (cinematicText != null)
        {
            cinematicText.text = "";
        }

        yield return StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOut()
    {
        float elapsed = 0f;

        fadeCanvas.blocksRaycasts = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            fadeCanvas.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            yield return null;
        }

        fadeCanvas.alpha = 1f;
    }

    public IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            fadeCanvas.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            yield return null;
        }

        fadeCanvas.alpha = 0f;
        fadeCanvas.blocksRaycasts = false;
    }
}