using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialOverlay;

    [SerializeField] private TMP_Text enemyTitle;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TMP_Text keybindText;
    [SerializeField] private TMP_Text flavorText;

    [Header("Enemy Display")]
    [SerializeField] private Enemy displayEnemyPrefab;
    [SerializeField] private Transform displayPoint;

    private Enemy displayEnemy;

    private HashSet<ElementType> seenElements = new();

    private Enemy currentTutorialEnemy;

    public bool TutorialActive { get; private set; }

    public Enemy CurrentTutorialEnemy => currentTutorialEnemy;

    private void Awake()
    {
        Instance = this;

        tutorialOverlay.SetActive(false);

        if (displayEnemyPrefab != null)
        {
            displayEnemy = Instantiate(
                displayEnemyPrefab,
                displayPoint.position,
                Quaternion.identity,
                displayPoint
            );

            displayEnemy.transform.localPosition = Vector3.zero;
            displayEnemy.transform.localRotation = Quaternion.identity;
            displayEnemy.transform.localScale = Vector3.one * 150f;

            displayEnemy.gameObject.SetActive(false);
        }
    }

    public void TryStartTutorial(Enemy enemy)
    {
        if (GameManager.IsPaused)
        {
            StartCoroutine(WaitAndRetry(enemy));
            return;
        }

        if (seenElements.Contains(enemy.Element))
            return;

        seenElements.Add(enemy.Element);

        currentTutorialEnemy = enemy;
        TargetManager.Instance?.SetTarget(enemy);

        TutorialActive = true;

        tutorialOverlay.SetActive(true);

        SetupTutorial(enemy);

        StartCoroutine(SlowTutorialIntro(enemy));
    }

    private void SetupTutorial(Enemy enemy)
    {
        if (displayEnemy != null)
        {
            displayEnemy.gameObject.SetActive(true);

            displayEnemy.SetVisualOnly(enemy.Element);
        }

        switch (enemy.Element)
        {
            case ElementType.Fire:

                enemyTitle.text = "FIRE SPRITE";

                instructionText.text =
                    "Fire enemies are weak to WATER";

                keybindText.text =
                    "Press E or 3";

                flavorText.text =
                    "\"Cool the flames before they spread.\"";

                break;

            case ElementType.Water:

                enemyTitle.text = "WATER SPRITE";

                instructionText.text =
                    "Water enemies are weak to LIGHTNING";

                keybindText.text =
                    "Press W or 2";

                flavorText.text =
                    "\"Electricity tears through unstable water mana.\"";

                break;

            case ElementType.Nature:

                enemyTitle.text = "NATURE SPRITE";

                instructionText.text =
                    "Nature enemies are weak to FIRE";

                keybindText.text =
                    "Press Q or 1";

                flavorText.text =
                    "\"Burn away the overgrowth.\"";

                break;

            case ElementType.Shadow:

                enemyTitle.text = "SHADOW SPRITE";

                instructionText.text =
                    "Shadow enemies are weak to RADIANT";

                keybindText.text =
                    "Press R or 4";

                flavorText.text =
                    "\"Light reveals what darkness conceals.\"";

                break;
        }
    }

    private IEnumerator SlowTutorialIntro(Enemy enemy)
    {
        yield return new WaitForSecondsRealtime(0.25f);

        Time.timeScale = 0f;

        GameManager.IsPaused = true;

        enemy.SetHighlight(true);
    }

    private IEnumerator WaitAndRetry(Enemy enemy)
    {
        while (GameManager.IsPaused)
        {
            yield return null;
        }

        TryStartTutorial(enemy);
    }

    public void CompleteTutorial()
    {
        TutorialActive = false;

        tutorialOverlay.SetActive(false);

        if (currentTutorialEnemy != null)
        {
            currentTutorialEnemy.SetHighlight(false);
        }

        if (displayEnemy != null)
        {
            displayEnemy.gameObject.SetActive(false);
        }

        Time.timeScale = 1f;

        GameManager.IsPaused = false;
    }
}