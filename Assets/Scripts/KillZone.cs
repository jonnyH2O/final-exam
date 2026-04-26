using UnityEngine;

public class KillZone : MonoBehaviour
{
    private Tower tower;

    private void Awake()
    {
        tower = FindFirstObjectByType<Tower>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has an Enemy component
        Enemy enemy = other.GetComponent<Enemy>(); 
        if (enemy != null)
        {
            tower.TakeDamage(1);
            enemy.Remove();
        }
    }
}