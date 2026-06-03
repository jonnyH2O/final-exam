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

        if (currentHP <= 0)
        {
            currentHP = 0;
            GameOver();
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RegisterDamage();
        }
    }

    private void GameOver()
    {
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowGameOver();
        }
    }
}