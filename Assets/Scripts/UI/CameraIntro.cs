using System.Collections;
using UnityEngine;

public class CameraIntro : MonoBehaviour
{
    [SerializeField] private Transform gameplayPoint;
    [SerializeField] private float moveDuration = 4f;
    [SerializeField] private WaveManager waveManager;

    private void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        GameManager.IsPaused = true;
        Time.timeScale = 0f;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = elapsed / moveDuration;

            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(
                startPosition,
                gameplayPoint.position,
                t
            );

            transform.rotation = Quaternion.Slerp(
                startRotation,
                gameplayPoint.rotation,
                t
            );

            yield return null;
        }

        transform.position = gameplayPoint.position;
        transform.rotation = gameplayPoint.rotation;

        Time.timeScale = 1f;

        GameManager.IsPaused = false;

        if (waveManager != null)
        {
            yield return new WaitForSecondsRealtime(1f);

            waveManager.StartWave();
        }
    }
}