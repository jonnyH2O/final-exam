using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SpellCaster : MonoBehaviour
{
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
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.BreakCombo();

            Debug.Log("Fizzle!"); // If wrong, fizzle
        }
    }
}