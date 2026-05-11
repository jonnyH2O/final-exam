using UnityEngine;
using UnityEngine.UI;

public class WaveProgressUI : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private int totalEnemies;
    private int enemiesDefeated;

    public void StartWave(int total)
    {
        totalEnemies = total;
        enemiesDefeated = 0;

        UpdateUI();
    }

    public void EnemyDefeated()
    {
        enemiesDefeated++;

        UpdateUI();
    }

    private void UpdateUI()
    {
        progressBar.value =
            (float)enemiesDefeated / totalEnemies;
    }
}