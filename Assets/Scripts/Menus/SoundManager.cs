using UnityEngine;
using UnityEngine.UI;

// Sets volume to 1 and saves it to PlayerPrefs when the game starts. When the slider is changed, it updates the volume and saves the new value to PlayerPrefs.
public class SoundManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Only set default if no saved value exists, so we don't overwrite the player's setting every Start
        if (!PlayerPrefs.HasKey("Volume"))
            PlayerPrefs.SetFloat("Volume", 1f);
        Load();
    }

    public void ChangeVolume()
    {
        // CHANGED: Guard against a missing slider reference
        if (volumeSlider == null) return;
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    // Reads the saved volume and applies it to the AudioListener and slider
    private void Load()
    {
        // CHANGED: read into a local with a fallback default of 1f
        float v = PlayerPrefs.GetFloat("Volume", 1f);
        // CHANGED: apply to AudioListener so volume works even in scenes without a slider
        AudioListener.volume = v;
        // CHANGED: null-check the slider before assigning (this fixed the NullReferenceException)
        if (volumeSlider != null)
            volumeSlider.value = v;
    }

    private void Save()
    {
        // CHANGED: skip saving when there's no slider to read from
        if (volumeSlider == null) return;
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }
}
