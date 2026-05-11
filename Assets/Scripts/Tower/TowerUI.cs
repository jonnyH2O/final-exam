using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TowerUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text hpText;

    private Tower tower;

    private void Start()
    {
        tower = FindFirstObjectByType<Tower>();

        // CHANGED: added a null check for the tower reference to prevent errors if it's missing
        if (tower == null)
        {
            Debug.LogWarning("TowerUI: No Tower found in scene; disabling.", this);
            enabled = false;
            return;
        }

        // CHANGED: guarded the health bar init so a missing inspector reference doesn't crash
        if (healthBar != null)
        {
            healthBar.maxValue = tower.MaxHP;
            healthBar.value = tower.CurrentHP;
        }
    }

    private void Update()
    {
        // CHANGED: bail out early if there's no tower so Update doesn't throw every frame
        if (tower == null) return;

        // CHANGED: only update the slider if it was assigned in the inspector
        if (healthBar != null) healthBar.value = tower.CurrentHP;

        // CHANGED: only update the HP text if it was assigned in the inspector
        if (hpText != null) hpText.text = $"{tower.CurrentHP} / {tower.MaxHP}";

        float percent = (float)tower.CurrentHP / tower.MaxHP;

        UpdateColor(percent);
    }

    private void UpdateColor(float percent)
    {
        // CHANGED: skip color updates if the fill image wasn't assigned
        if (fillImage == null) return;

        if (percent <= 0.2f)
        {
            fillImage.color = Color.red; // force red when critical
        }
        else
        {
            fillImage.color = Color.Lerp(Color.red, Color.green, percent);
        }
    }
    
}