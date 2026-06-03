using UnityEngine;
using UnityEngine.Video;

public class EasterEgg : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject videoPanel;

    private int clickCount;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPanel.SetActive(false);
    }

    public void SecretClick()
    {
        clickCount++;

        if (clickCount >= 3)
        {
            PlaySecretVideo();
        }
    }

    private void PlaySecretVideo()
    {
        clickCount = 0;

        menuUI.SetActive(false);

        videoPanel.SetActive(true);
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoPlayer.Stop();

        videoPanel.SetActive(false);
        menuUI.SetActive(true);
    }
}