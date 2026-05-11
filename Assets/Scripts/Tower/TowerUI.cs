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

        healthBar.maxValue = tower.MaxHP;
        healthBar.value = tower.CurrentHP;
    }

    private void Update()
    {
        healthBar.value = tower.CurrentHP;

        hpText.text = $"{tower.CurrentHP} / {tower.MaxHP}";

        float percent = (float)tower.CurrentHP / tower.MaxHP;

        UpdateColor(percent);
    }

    private void UpdateColor(float percent)
    {
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