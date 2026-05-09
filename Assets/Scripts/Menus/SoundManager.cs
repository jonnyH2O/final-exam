using UnityEngine;
using UnityEngine.UI;

// Sets volume to 1 and saves it to PlayerPrefs when the game starts. When the slider is changed, it updates the volume and saves the new value to PlayerPrefs.
public class SoundManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPrefs.SetFloat("Volume", 1f); // Set default volume to max
        Load();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume"); // Default volume is 1 (max)
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);   
    }
}
