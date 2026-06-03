using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EndingCutscene : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        videoPlayer.loopPointReached += VideoFinished;
        fadeCanvas.alpha = 0f;
    }

    private void VideoFinished(VideoPlayer vp)
    {
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene("MainMenu");
    }
}