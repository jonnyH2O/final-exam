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
        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.qKey.wasPressedThisFrame) cast = SpellType.Fire;
        if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) cast = SpellType.Lightning;
        if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame) cast = SpellType.Water;
        if (Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.rKey.wasPressedThisFrame) cast = SpellType.Radiant;
        if (cast == null) return;

        // Get the current target from the TargetManager
        Enemy target = TargetManager.Instance.CurrentTarget; 
        if (target == null) return;

        // Check if the cast spell counters the target's element
        SpellType required = counters[target.Element];
        if (cast == required)
            target.Remove(); // If correct spell is cast, remove the enemy
        else
            Debug.Log("Fizzle!"); // If wrong, fizzle
    }
}