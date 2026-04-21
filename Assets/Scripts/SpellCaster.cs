using UnityEngine;
using System.Collections.Generic; // For the element counter dictionary
using UnityEngine.InputSystem; // For the new Input System
public class SpellCaster : MonoBehaviour
{
    // The element chart, one counter per enemy element
private Dictionary<ElementType, SpellType> counters = new()
{
    { ElementType.Nature,  SpellType.Fire },
    { ElementType.Fire,    SpellType.Water },
    { ElementType.Water,   SpellType.Lightning },
    { ElementType.Shadow,  SpellType.Radiant }, 
};

    private void Update()
    {
        SpellType? cast = null; // Null means no cast

        // Check for cast input (number keys or QWER) and set the cast element 
        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.qKey.wasPressedThisFrame) cast = SpellType.Fire;
        if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) cast = SpellType.Lightning;
        if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame) cast = SpellType.Water;
        if (Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.rKey.wasPressedThisFrame) cast = SpellType.Radiant;
        if (cast == null) return;

        // Look up the current target from the TargetManager singleton
        Enemy target = TargetManager.Instance.CurrentTarget; 
        if (target == null) return;

        // Look up the required counter element for the target's element and compare to the cast element
        SpellType required = counters[target.Element];
        if (cast == required)
            Destroy(target.gameObject); // correct, kill it
        else
            Debug.Log("Fizzle!"); // wrong element, fizzle the spell (no effect for now)
    }
}