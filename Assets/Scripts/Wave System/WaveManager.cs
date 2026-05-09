using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private List<WaveData> waves = new();
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private TextMeshProUGUI waveMessageText;
    
    private int _currentWaveIndex = 0;
    private int _activeEnemyCount = 0;
    private int _remaining = 0;
    
    public void StartWave()
    {
        StartCoroutine(NextWaveDelay(waves[_currentWaveIndex], 2f));
    }

    private IEnumerator NextWaveDelay(WaveData data, float overrideDelay = -1f)
    {
        if (!string.IsNullOrEmpty(data.waveStartMessage)) // Show wave start message if it exists
            StartCoroutine(ShowWaveMessage(data.waveStartMessage));

        // Use the override delay if provided, otherwise use the wave's default delay
        float delay = overrideDelay >= 0f ? overrideDelay : waves[_currentWaveIndex - 1].waveDelay; 
        Debug.Log($"Next wave in {delay} seconds...");
        yield return new WaitForSeconds(delay); 
        StartCoroutine(SpawnWave(data)); 
    }


    private IEnumerator SpawnWave(WaveData data)
    {
        // Build the spawn queue based on the wave data and shuffle it
        List<EnemyType> queue = BuildSpawnQueue(data);
        _remaining = queue.Count;
        int toSpawn = queue.Count;
        int index = 0;
        
        // Spawn enemies at intervals until the queue is empty
        while (toSpawn > 0)
        {
            SpawnEnemy(queue[index], data);
            index++;
            toSpawn--;
            yield return new WaitForSeconds(data.spawnInterval);
        }
    }
 
    private List<EnemyType> BuildSpawnQueue(WaveData data)
    {
        // Create a list of enemy types and counts to spawn based on wave data
        var queue = new List<EnemyType>();
        foreach (var entry in data.enemyEntries)
            for (int i = 0; i < entry.count; i++)
                queue.Add(entry.enemyType); 

        // Fisher-Yates shuffle (in-place, O(n) time complexity)
        for (int i = queue.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (queue[i], queue[j]) = (queue[j], queue[i]);
        }
        return queue;
    }

    private void SpawnEnemy(EnemyType type, WaveData data)
    {
        // Get a random spawn point and an enemy from the pool, then put it at the spawn point
        int lane = Random.Range(0, spawnPoints.Length);
        GameObject obj = EnemyPool.Instance.Get(type);
        obj.transform.position = spawnPoints[lane].position;
        
        // Configure the enemy's element, speed, and type based on the wave data
        Enemy enemy = obj.GetComponent<Enemy>();
        ElementType element = data.allowedElements[Random.Range(0, data.allowedElements.Count)];
        enemy.SetEnemyType(type);
        enemy.Initialize(element, data.enemySpeed, spawnPoints[lane].position);

        _activeEnemyCount++;
    }

    public void NotifyEnemyRemoved(GameObject obj)
    {
        // Read the type from the enemy so we return it to the correct pool
        EnemyType type = obj.GetComponent<Enemy>().EnemyType; 
        EnemyPool.Instance.Return(type, obj); 
        _activeEnemyCount--;
        _remaining--;

        // If all enemies for the current wave have been removed, start the next wave after a delay
        if (_remaining <= 0)
        {
            _currentWaveIndex++;
            if (_currentWaveIndex >= waves.Count)
                Debug.Log("All waves complete!");
            else
            {
                Debug.Log($"Wave cleared! Starting wave {_currentWaveIndex + 1}.");
                StartCoroutine(NextWaveDelay(waves[_currentWaveIndex]));
            }
        }
    }
    
    private IEnumerator ShowWaveMessage(string message)
    {
        waveMessageText.text = message;
        waveMessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        waveMessageText.gameObject.SetActive(false);
    }
}