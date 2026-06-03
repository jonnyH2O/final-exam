using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Makes a GUI visual element aspect of the Fizzle functionality 
public class SpellSlotUI : MonoBehaviour
{
    [Header("Which Spell")]
    [Tooltip("Which spell this slot represents.")]
    [SerializeField] private SpellType spell;

    [Header("References")]
    [Tooltip("Background panel of the slot")]
    [SerializeField] private Image background;

    [Tooltip("Spell icon image")]
    [SerializeField] private Image icon;

    [Tooltip("Keybind text. Shows the bind, countdown when fizzled.")]
    [SerializeField] private TMP_Text keybindText;

    [Header("Display")]
    [Tooltip("Text shown when spell is castable")]
    [SerializeField] private string keybindLabel = "Q";

    [Header("Colors")]
    [SerializeField] private Color normalBackgroundColor = Color.white;
    [SerializeField] private Color normalIconColor = Color.white;
    [Tooltip("Color applied to both background and icon when locked")]
    [SerializeField] private Color lockedColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private bool _wasLocked = false;

    private void Start()
    {
        ApplyUnlocked();
    }

    private void Update()
    {
        if (SpellCaster.Instance == null) return;

        float remaining = SpellCaster.Instance.GetLockoutRemaining(spell);
        bool isLocked = remaining > 0f;

        if (isLocked)
        {
            if (!_wasLocked) ApplyLocked();
            if (keybindText != null)
                keybindText.text = remaining.ToString("F1");
        }
        else if(_wasLocked)
        {
            ApplyUnlocked();
        }

        _wasLocked = isLocked;
    }

    private void ApplyLocked()
    {
        if (background != null) background.color = lockedColor;
        if (icon != null) icon.color = new Color (1f, 1f, 1f,  0.25f);
    }

    private void ApplyUnlocked()
    {
        if (background != null) background.color = normalBackgroundColor;
        if (icon != null) icon.color = normalIconColor;
        if (keybindText != null) keybindText.text = keybindLabel;
    }
}
