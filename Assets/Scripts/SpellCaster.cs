using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SpellCaster : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("AudioSource used to play SFX from this script. Should be 2D (Spatial Blend = 0).")]
    [SerializeField] private AudioSource sfxSource;

    [Tooltip("Sound played when the player casts the wrong spell.")]
    [SerializeField] private AudioClip fizzleClip;

    [Header("Fizzle Lockout")]
    [Tooltip("Seconds that spell input is ignored after a wrong cast.")]
    [SerializeField] private float fizzleLockoutDuration = 3f;

    
    private Dictionary<SpellType, float> _spellLockouts = new();
    // Define the counter relationships between elements and spells
    private Dictionary<ElementType, SpellType> counters = new()
    {
        { ElementType.Nature,  SpellType.Fire },
        { ElementType.Fire,    SpellType.Water },
        { ElementType.Water,   SpellType.Lightning },
        { ElementType.Shadow,  SpellType.Radiant },
    };

    private void Update()
    {
        SpellType? cast = null; // Nullable type to represent no spell cast

        // Check for spell cast input (number keys or QWER)
        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.qKey.wasPressedThisFrame)
            cast = SpellType.Fire;

        if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
            cast = SpellType.Lightning;

        if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame)
            cast = SpellType.Water;

        if (Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.rKey.wasPressedThisFrame)
            cast = SpellType.Radiant;

        if (cast == null)
            return;

        Enemy target = null;

        if (TutorialManager.Instance != null &&
            TutorialManager.Instance.TutorialActive)
        {
            target = TutorialManager.Instance.CurrentTutorialEnemy;
        }
        else if (TargetManager.Instance != null)
        {
            target = TargetManager.Instance.CurrentTarget;
        }

        if (target == null)
            return;
        if (IsSpellLocked(cast.Value)) return;

        // target was already assigned above
        if (target == null) return;

        // Check if the cast spell counters the target's element
        if (!counters.TryGetValue(target.Element, out SpellType required))
            return;

        if (cast == required)
        {
            bool tutorialKill =
                TutorialManager.Instance != null &&
                TutorialManager.Instance.TutorialActive;

            target.Remove(); // If correct spell is cast, remove the enemy

            // Only award score/combo for normal enemies, not tutorial enemies
            if (!tutorialKill && ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddKill();
            }

            // End the tutorial after the tutorial enemy is defeated
            if (tutorialKill)
            {
                TutorialManager.Instance.CompleteTutorial();
            }
        }
        else
        {   
            // During Tutorial, block wrong input entirely, prevents all fizzle mechanics.
            if (TutorialManager.Instance != null && TutorialManager.Instance.TutorialActive)
                return;

            if (ScoreManager.Instance != null)
                ScoreManager.Instance.BreakCombo();

            Debug.Log("Fizzle!"); // If wrong, fizzle
            // Wrong spell, triggers a fizzle which starts, or restarts, lockout
            Fizzle(required);
        }

    }

    private bool IsSpellLocked(SpellType spell)
    {
        return _spellLockouts.TryGetValue(spell, out float endTime) && Time.time < endTime;
    }

    private void Fizzle(SpellType spellToLock)
    {
        
        // Start input lockout on correct spell
        _spellLockouts[spellToLock] = Time.time + fizzleLockoutDuration;

        // Play the fizzle SFX.
        if (sfxSource != null && fizzleClip != null)
            sfxSource.PlayOneShot(fizzleClip);

        Debug.Log("Fizzle! {spellToLock} locked for {fizzleLockoutDuration}s."); // If wrong, fizzle
    }
}