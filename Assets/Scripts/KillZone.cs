using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has an Enemy component
        Enemy enemy = other.GetComponent<Enemy>(); 
        if (enemy != null)
            enemy.Remove();
    }
}