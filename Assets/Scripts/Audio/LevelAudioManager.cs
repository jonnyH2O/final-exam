using System.Collections;
using UnityEngine;


/// <summary>
/// Allows for the per-level audio to work properly. Loops Ambience
/// and music throughout the level. Ambience starts immediately on scene load
/// then music fades in once cinematic is over, allowing for player settlement
/// 
/// Setup:
///     - Placed on an empty GameObject ("LevelAudio").
///     - Two AudioSource Components, one per track
///     - Drag the matching AudioSources to the proper field
///     - Both AudioSources have "Loop" enabled and "Play on Awake" Disabled
///         This scrip controls their playback
/// </summary>
public class LevelAudioManager : MonoBehaviour
{

    public static LevelAudioManager Instance;

    [Header("AudioSources")]
    [Tooltip("Looping ambience track - Leave empty if this level has no ambience")]
    [SerializeField] private AudioSource ambienceSource;

    [Tooltip("Looping music track for level")]
    [SerializeField] private AudioSource musicSource;

    [Header("Timing")]
    [Tooltip("Seconds from scene start until music begins fading in. Change this with level cinematic")]
    [SerializeField] private float musicStartDelay = 5f;

    [Header("Fade-In Durations")]
    [Tooltip("Time for ambience to ramp from silent to target volume")]
    [SerializeField] private float ambienceFadeInDuration = 1.5f;
    
    [Tooltip("Time for music to ramp from silent to target volume")]
    [SerializeField] private float musicFadeInDuration = 2.5f;

    [Header("Fade out durations")]
    [Tooltip("Time for ambience to fade out")]
    [SerializeField] private float ambienceFadeOutDuration = 3f;

    [Tooltip("Time for music to fade out")]
    [SerializeField] private float musicFadeOutDuration = 3f;

    [Header("Target Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float ambienceTargetVolume = 0.6f;

    [Range(0f, 1f)]
    [SerializeField] private float musicTargetVolume = 0.7f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
        if (ambienceSource != null) ambienceSource.volume = 0f;
        if (musicSource != null) musicSource.volume = 0f;

        StartCoroutine(PlayLevelAudio());
    }

    private IEnumerator PlayLevelAudio()
    {
        
        if (ambienceSource != null && ambienceSource.clip != null)
        {
            ambienceSource.Play();
            StartCoroutine(FadeIn(ambienceSource, ambienceTargetVolume, ambienceFadeInDuration));
        }

        yield return new WaitForSeconds(musicStartDelay);

        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.Play();
            StartCoroutine(FadeIn(musicSource, musicTargetVolume, musicFadeInDuration));
        }
    }

    private IEnumerator FadeIn(AudioSource source, float targetVolume, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }
        source.volume = targetVolume;
    }

    public void FadeOutAll()
    {
        if (ambienceSource != null && ambienceSource.isPlaying)
            StartCoroutine(FadeOut(ambienceSource, ambienceFadeOutDuration));

        if (musicSource != null && musicSource.isPlaying)
            StartCoroutine(FadeOut(musicSource, musicFadeOutDuration));
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume,0f, elapsed / duration);
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
    }
}
