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

    [Tooltip("Sound played when Fire is cast correctly (against Nature enemies).")]
    [SerializeField] private AudioClip fireClip;

    [Tooltip("Sound played when Lightning is cast correctly (against Water enemies).")]
    [SerializeField] private AudioClip lightningClip;

    [Tooltip("Sound played when Water is cast correctly (against Fire enemies).")]
    [SerializeField] private AudioClip waterClip;

    [Tooltip("Sound played when Radiant is cast correctly (against Shadow enemies).")]
    [SerializeField] private AudioClip radiantClip;
    
    [Tooltip("Sound played when a Shielded enemy's shield is broken (not when it dies).")]
    [SerializeField] private AudioClip shieldBreakClip;

    [Header("Fizzle Lockout")]
    [Tooltip("Seconds that spell input is ignored after a wrong cast.")]
    [SerializeField] private float fizzleLockoutDuration = 3f;

    [Header("Fizzle VFX")]
    [Tooltip("Effect spawned on the target when the wrong spell is cast (e.g. Smoke puff).")]
    [SerializeField] private GameObject fizzleEffect;
    [Tooltip("Scale multiplier for the fizzle effect. Below 1 = smaller puff.")]
    [SerializeField] private float fizzleEffectScale = 0.5f;
    [Tooltip("Playback speed for the fizzle effect. Above 1 = faster.")]
    [SerializeField] private float fizzleEffectSpeed = 2f;

    
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
            // Clears any active fizzles to prevent softlock
            _spellLockouts.Clear();
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

        // Work out the spell this target needs. SplitEnemy accepts any spell.
        counters.TryGetValue(target.Element, out SpellType required);
        bool accepted = !target.RequiresMatchingSpell || cast == required;

        if (accepted)
        {
            bool tutorialKill =
                TutorialManager.Instance != null &&
                TutorialManager.Instance.TutorialActive;

            // Some enemies survive a correct hit (shield) or transform without
            // dying (split). Only score / finish the tutorial on a real kill.
            bool killed = target.TakeCorrectHit(cast.Value);

            if (sfxSource != null)
            {
                if (killed || target.EnemyType == EnemyType.Split)
                {
                    // Play spell specific sound on an Enemy Death OR Enemy Split
                    // Fizzle sound is on wrong cast only, shouldn't overlap
                    AudioClip spellClip = GetClipForSpell(cast.Value);
                    if (spellClip != null)
                        sfxSource.PlayOneShot(spellClip);
                }
                else if (target.EnemyType == EnemyType.Shielded)
                {
                    // Shield is broken, enemy alive, only play shield break
                    if (shieldBreakClip != null)
                        sfxSource.PlayOneShot(shieldBreakClip);
                }
                
            }

            // Only award score/combo for normal enemies, not tutorial enemies
            if (killed && !tutorialKill && ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddKill();
            }

            // End the tutorial after the tutorial enemy is defeated
            if (killed && tutorialKill)
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

            // Quick, small smoke puff on the target to telegraph the miss.
            target.PlayCenteredEffect(fizzleEffect, fizzleEffectScale, fizzleEffectSpeed);

            Debug.Log("Fizzle!"); // If wrong, fizzle
            // Wrong spell, triggers a fizzle which starts, or restarts, lockout
            Fizzle(required);
        }

    }

    private AudioClip GetClipForSpell(SpellType spell)
    {
        return spell switch
        {
            SpellType.Fire => fireClip,
            SpellType.Lightning => lightningClip,
            SpellType.Water => waterClip,
            SpellType.Radiant => radiantClip,
            _                   => null
        };
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