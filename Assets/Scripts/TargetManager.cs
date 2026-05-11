using UnityEngine;
using UnityEngine.InputSystem;

// Handles mouse-based enemy targeting via raycasting
public class TargetManager : MonoBehaviour
{
    // Singleton 
    public static TargetManager Instance { get; private set; } 

    // Private Fields 
    private Enemy currentTarget;

    // Public Properties 
    public Enemy CurrentTarget => currentTarget;

    // Unity Lifecycle 
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Cast a ray from the camera through the mouse cursor each frame
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // Will be null if the hit object has no Enemy component
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            SetTarget(enemy);
        }
        else
        {
            SetTarget(null);
        }
    }

    // Private Methods
    public void SetTarget(Enemy newTarget)
    {
        // Swaps the active target and updates highlight state on both old and new targets
        if (newTarget == currentTarget) return;

        if (currentTarget != null)
            currentTarget.SetHighlight(false);

        currentTarget = newTarget;

        if (currentTarget != null)
            currentTarget.SetHighlight(true);
    }
}
