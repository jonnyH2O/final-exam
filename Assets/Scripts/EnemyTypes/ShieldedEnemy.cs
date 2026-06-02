using UnityEngine;

// Takes two correct hits: the first strips the shield, the second kills it.
public class ShieldedEnemy : Enemy
{
    [Header("Shield")]
    [Tooltip("Magic shield PREFAB. Instantiated as a child of the enemy while " +
             "shielded (the prefab already loops, so it persists until broken).")]
    [SerializeField] private GameObject shieldVisual;
    [Tooltip("Local offset for the shield, e.g. raise it to the enemy's center.")]
    [SerializeField] private Vector3 shieldOffset = Vector3.zero;
    [Tooltip("Scale multiplier for the shield. 0.5 = half size.")]
    [SerializeField] private float shieldScale = 0.625f;
    [Tooltip("One-shot effect played when the shield breaks (e.g. Sparks explode blue).")]
    [SerializeField] private GameObject shieldBreakEffect;
    [Tooltip("World offset for the break effect from the enemy center. " +
             "+Y lifts it on top; adjust X/Z to push it in front toward the camera.")]
    [SerializeField] private Vector3 shieldBreakOffset = new Vector3(0f, 0.5f, 0f);

    private GameObject _shieldInstance;
    private bool _shielded;

    protected override void OnInitialized()
    {
        ShowShield(); // re-arm on every spawn, including pool reuse
    }

    public override bool TakeCorrectHit()
    {
        if (_shielded)
        {
            HideShield(); // first correct hit breaks the shield; enemy lives
            return false;
        }

        return base.TakeCorrectHit(); // second correct hit kills
    }

    private void ShowShield()
    {
        _shielded = true;
        if (shieldVisual == null) return;

        // Spawn the shield as a child once, then reuse it across pool respawns.
        if (_shieldInstance == null)
        {
            _shieldInstance = Instantiate(shieldVisual, transform);
            _shieldInstance.transform.localPosition = shieldOffset;
            // Shield systems use Hierarchy scaling mode, so this shrinks them.
            _shieldInstance.transform.localScale = shieldVisual.transform.localScale * shieldScale;
        }

        _shieldInstance.SetActive(true);

        // Force the looping shield to (re)start so it's visible immediately.
        foreach (ParticleSystem ps in _shieldInstance.GetComponentsInChildren<ParticleSystem>())
            ps.Play(true);
    }

    private void HideShield()
    {
        _shielded = false;
        if (_shieldInstance != null)
            _shieldInstance.SetActive(false);

        // Burst of sparks as the shield shatters, offset so it reads in front.
        PlayCenteredEffect(shieldBreakEffect, offset: shieldBreakOffset);
    }
}
