using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;

    private int score;
    private int combo;
    private int maxCombo;

    private bool tookDamage;
    private bool enemiesFizzled;

    public bool FullClear => !tookDamage && !enemiesFizzled;
    public int Score => score;
    public int MaxCombo => maxCombo;

    private void Awake()
    {
        Instance = this;
        ResetRun();
    }

    private void OnEnable()
    {
        ResetRun();
    }

    public void ResetRun()
    {
        score = 0;
        combo = 0;
        maxCombo = 0;

        tookDamage = false;
        enemiesFizzled = false;

        UpdateUI();
    }

    public void AddKill()
    {
        combo++;

        if (combo > maxCombo)
            maxCombo = combo;

        score += 100 + (combo * 10);

        UpdateUI();
    }

    public void BreakCombo()
    {
        combo = 0;
        UpdateUI();
    }

    public void RegisterDamage()
    {
        tookDamage = true;
        BreakCombo();
    }

    public void RegisterFizzledEnemy()
    {
        enemiesFizzled = true;
        BreakCombo();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";

        if (comboText != null)
            comboText.text = $"Combo x{combo}";
    }
}