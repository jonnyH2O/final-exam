using TMPro;
using UnityEngine;
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

    private HashSet<ElementType> seenElements = new();

    private Enemy currentTutorialEnemy;

    public bool TutorialActive { get; private set; }

    public Enemy CurrentTutorialEnemy => currentTutorialEnemy;

    private void Awake()
    {
        Instance = this;

        tutorialOverlay.SetActive(false);
    }

    public void TryStartTutorial(Enemy enemy)
    {
        if (seenElements.Contains(enemy.Element))
            return;

        seenElements.Add(enemy.Element);

        currentTutorialEnemy = enemy;
        TargetManager.Instance?.SetTarget(enemy);

        TutorialActive = true;

        tutorialOverlay.SetActive(true);

        SetupTutorial(enemy);

        GameManager.IsPaused = true;

        enemy.SetHighlight(true);
    }

    private void SetupTutorial(Enemy enemy)
    {
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

    public void CompleteTutorial()
    {
        TutorialActive = false;

        tutorialOverlay.SetActive(false);

        GameManager.IsPaused = false;
    }
}