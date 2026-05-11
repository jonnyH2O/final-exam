using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnEntry
{
    public EnemyType enemyType;
    public int count;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "FinalExam/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("Tutorial Intro")]
    public bool hasGuaranteedIntroEnemy;
    public ElementType guaranteedIntroElement;

    [Header("Random Pool")]
    public ElementType[] randomElementPool;

    [Header("Enemies")]
    public List<EnemySpawnEntry> enemyEntries = new();

    [Header("Spawn Settings")]
    public float enemySpeed = 2f;
    public float spawnInterval = 2f;

    [Header("Wave Settings")]
    public float waveDelay = 3f;

    [Header("Wave UI")]
    [TextArea]
    public string waveStartMessage;
}