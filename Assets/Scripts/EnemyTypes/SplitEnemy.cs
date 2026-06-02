using UnityEngine;

// Breaks apart on ANY spell into two smaller basic enemies with random
// elements. Conceptually one enemy that's "actually three" — the parent break
// isn't a scored kill; killing the two children is.
public class SplitEnemy : Enemy
{
    [Header("Split")]
    [Tooltip("Basic enemy prefab instantiated for each child (NOT pooled).")]
    [SerializeField] private GameObject childPrefab;
    [SerializeField] private int childCount = 2;
    [SerializeField] private float childScale = 0.6f;
    [Tooltip("Front/back (X) spacing between the spawned children.")]
    [SerializeField] private float childSpread = 1.5f;
    [Tooltip("Elements the children can randomly take.")]
    [SerializeField] private ElementType[] childElementPool = new ElementType[]
    {
        ElementType.Fire, ElementType.Water, ElementType.Nature, ElementType.Shadow
    };

    // Any spell cracks the split sprite open.
    public override bool RequiresMatchingSpell => false;

    // Keep the prefab's own (non-elemental) material so its colors signal that
    // any spell can be used on it.
    protected override void ApplyElementMaterial() { }

    public override bool TakeCorrectHit(SpellType castSpell)
    {
        SpawnChildren();
        PlayHitVfxForSpell(castSpell);
        Remove(); // play this enemy's hit VFX and remove itself
        return false; // the break itself isn't a scored kill; the children are
    }

    private void SpawnChildren()
    {
        if (childPrefab == null) return;

        for (int i = 0; i < childCount; i++)
        {
            // Spread children in front of and behind the parent (movement axis).
            Vector3 pos = transform.position;
            pos.x += (i - (childCount - 1) * 0.5f) * childSpread;

            GameObject child = Instantiate(childPrefab, pos, transform.rotation);
            child.transform.localScale = childPrefab.transform.localScale * childScale;

            Enemy enemy = child.GetComponent<Enemy>();
            if (enemy == null) continue;

            enemy.SetEnemyType(EnemyType.Basic);
            enemy.IsPooled = false; // not pooled -> destroyed on removal

            ElementType element = childElementPool[Random.Range(0, childElementPool.Length)];
            enemy.Initialize(element, speed, pos);

            // Keep the wave counter/progress accurate for the new bodies.
            _waveManager?.NotifyEnemySpawned();
        }
    }
}
