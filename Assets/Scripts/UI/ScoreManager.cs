using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
            scoreText.text = $"{score}pts";

        if (comboText != null)
            comboText.text = $"Combo x{combo}";
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        scoreText = GameObject.FindWithTag("ScoreText")?.GetComponent<TMP_Text>();
        comboText = GameObject.FindWithTag("ComboText")?.GetComponent<TMP_Text>();

        UpdateUI();
    }
}