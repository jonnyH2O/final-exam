using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private List<WaveData> waves = new();
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private TextMeshProUGUI waveMessageText;
    [SerializeField] private WaveProgressUI waveProgressUI;

    private int _currentWaveIndex = 0;

    private int _activeEnemyCount = 0;
    private int _remaining = 0;
    private bool _waveTransitioning;
    private bool _waveActive;

    public void StartWave()
    {
        // Prevent starting a new wave while one is already active or transitioning
        if (_waveActive || _waveTransitioning)
        {
            return;
        }

        // Ensure wave data exists before starting
        if (waves == null || waves.Count == 0)
        {
            Debug.LogError("No waves assigned.");
            return;
        }

        // Reset to the first wave if the index exceeds the list
        if (_currentWaveIndex >= waves.Count)
        {
            _currentWaveIndex = 0;
        }

        StartCoroutine(NextWaveDelay(waves[_currentWaveIndex], _currentWaveIndex, 2f));
    }

    private IEnumerator NextWaveDelay(WaveData data, int waveIndex, float overrideDelay = -1f)
    {
        // Show wave start message if it exists
        if (!string.IsNullOrEmpty(data.waveStartMessage))
            StartCoroutine(ShowWaveMessage(data.waveStartMessage));

        // Use the override delay if provided, otherwise use the previous wave's delay
        float delay = overrideDelay >= 0f
            ? overrideDelay
            : (waveIndex > 0 ? waves[waveIndex - 1].waveDelay : 0f);

        Debug.Log($"Next wave in {delay} seconds...");

        yield return new WaitForSeconds(delay);

        StartCoroutine(SpawnWave(data, waveIndex));
    }

    private IEnumerator SpawnWave(WaveData wave, int waveIndex)
    {
        _waveActive = true;
        _waveTransitioning = false;

        // Build the spawn queue based on the wave data and shuffle it
        List<SpawnData> queue = BuildSpawnQueue(wave, waveIndex);

        _remaining = queue.Count;

        // Initialize the wave progress UI with the total enemies remaining
        if (waveProgressUI != null)
        {
            waveProgressUI.StartWave(_remaining);
        }

        // End early if there are no enemies to spawn
        if (_remaining <= 0)
        {
            _waveActive = false;
            yield break;
        }

        // Spawn enemies at intervals until the queue is empty
        for (int i = 0; i < queue.Count; i++)
        {
            SpawnEnemy(queue[i].enemyType, queue[i].elementType, wave);

            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private List<SpawnData> BuildSpawnQueue(WaveData wave, int waveIndex)
    {
        // Create a list of enemy types and elements to spawn based on wave data
        var queue = new List<SpawnData>();

        // Add a guaranteed intro enemy if enabled
        if (wave.hasGuaranteedIntroEnemy)
        {
            queue.Add(new SpawnData
            {
                enemyType = EnemyType.Basic,
                elementType = wave.guaranteedIntroElement
            });
        }

        // Add all enemies from the wave entries with randomized elements
        foreach (var entry in wave.enemyEntries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                ElementType randomElement =
                    wave.randomElementPool[
                        Random.Range(0, wave.randomElementPool.Length)
                    ];

                queue.Add(new SpawnData
                {
                    enemyType = entry.enemyType,
                    elementType = randomElement
                });
            }
        }

        // Skip the intro enemy during shuffling so it always spawns first
        int startIndex = wave.hasGuaranteedIntroEnemy ? 1 : 0;

        // Fisher-Yates shuffle (in-place, O(n) time complexity)
        for (int i = queue.Count - 1; i > startIndex; i--)
        {
            int j = Random.Range(startIndex, i + 1);
            (queue[i], queue[j]) = (queue[j], queue[i]);
        }

        return queue;
    }

    private void SpawnEnemy(EnemyType type, ElementType element, WaveData data)
    {
        // Ensure spawn points exist before attempting to spawn
        if (spawnPoints == null || spawnPoints.Length == 0)
            return;

        // Ensure the enemy pool exists before requesting an enemy
        if (EnemyPool.Instance == null)
            return;

        // Get a random spawn point and an enemy from the pool, then place it at the spawn point
        int lane = Random.Range(0, spawnPoints.Length);

        GameObject obj = EnemyPool.Instance.Get(type);

        if (obj == null)
            return;

        obj.transform.position = spawnPoints[lane].position;

        Enemy enemy = obj.GetComponent<Enemy>();

        if (enemy == null)
            return;

        // Configure the enemy's element, speed, and type based on the wave data
        enemy.SetEnemyType(type);
        enemy.Initialize(element, data.enemySpeed, spawnPoints[lane].position);

        _activeEnemyCount++;
    }

    public void NotifyEnemyRemoved(GameObject obj)
    {
        // Ignore removal if no wave is active or the object is invalid
        if (!_waveActive || obj == null)
            return;

        Enemy enemyComponent = obj.GetComponent<Enemy>();

        if (enemyComponent == null)
            return;

        // Read the type from the enemy so we return it to the correct pool
        EnemyType type = enemyComponent.EnemyType;

        if (EnemyPool.Instance != null)
        {
            EnemyPool.Instance.Return(type, obj);
        }

        _activeEnemyCount--;
        _remaining--;

        // Update the wave progress UI when an enemy is defeated
        if (waveProgressUI != null)
        {
            waveProgressUI.EnemyDefeated();
        }

        // If all enemies for the current wave have been removed, start the next wave after a delay
        if (_remaining <= 0 && !_waveTransitioning)
        {
            _waveTransitioning = true;
            _waveActive = false;

            _currentWaveIndex++;

            if (_currentWaveIndex >= waves.Count)
            {
                Debug.Log("All waves complete!");

                // Show the level complete UI if all waves are finished
                if (GameUIManager.Instance != null)
                {
                    GameUIManager.Instance.ShowLevelComplete();
                }
            }
            else
            {
                Debug.Log($"Wave cleared! Starting wave {_currentWaveIndex + 1}.");

                StartCoroutine(NextWaveDelay(waves[_currentWaveIndex], _currentWaveIndex));
            }
        }
    }

    private IEnumerator ShowWaveMessage(string message)
    {
        // Ensure the wave message UI exists before showing text
        if (waveMessageText == null)
            yield break;

        waveMessageText.text = message;
        waveMessageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        waveMessageText.gameObject.SetActive(false);
    }
}

[System.Serializable]
public struct SpawnData
{
    public EnemyType enemyType;
    public ElementType elementType;
}