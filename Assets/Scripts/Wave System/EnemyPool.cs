using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; } 

    [System.Serializable]
    public class PoolEntry
    {
        public EnemyType enemyType;
        public GameObject prefab;
        public int defaultCapacity = 10;
        public int maxSize = 20;
    }

    // List of pool entries to set up in the inspector, and a dictionary to hold the actual pools
    [SerializeField] private List<PoolEntry> poolEntries = new(); 
    private Dictionary<EnemyType, ObjectPool<GameObject>> _pools = new();

    private void Awake()
    {
        Instance = this;

        foreach (var entry in poolEntries)
        {
            var prefab = entry.prefab;
            _pools[entry.enemyType] = new ObjectPool<GameObject>(
                createFunc:      ()  => Instantiate(prefab),
                actionOnGet:     obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: entry.defaultCapacity,
                maxSize:         entry.maxSize
            );
        }
    }

    public GameObject Get(EnemyType type) => _pools[type].Get(); // Get an enemy from the pool of the specified type

    public void Return(EnemyType type, GameObject obj) => _pools[type].Release(obj); // Return an enemy to the pool of the specified type
}