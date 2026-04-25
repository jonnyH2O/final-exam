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
    [Header("Enemies")]
    public List<EnemySpawnEntry> enemyEntries = new();

    [Header("Allowed Elements")]
    public List<ElementType> allowedElements;

    [Header("Spawn Settings")]
    public float enemySpeed = 2f;
    public float spawnInterval = 2f;

    [Header("Wave Settings")]
    public float waveDelay = 3f;

    [Header("Wave UI")]
    public string waveStartMessage;
}