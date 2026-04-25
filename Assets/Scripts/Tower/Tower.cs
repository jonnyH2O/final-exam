using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;
    private int currentHP;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        Debug.Log("Tower took damage! HP: " + currentHP);

        if (currentHP <= 0)
        {
            currentHP = 0;
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        // hook into GameManager
        Time.timeScale = 0f; // pause game for now
    }
}