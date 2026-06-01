using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private ElementType element;
    [SerializeField] private float speed = 2f;

    [SerializeField] private Material _fireMaterial;
    [SerializeField] private Material _waterMaterial;
    [SerializeField] private Material _natureMaterial;
    [SerializeField] private Material _shadowMaterial;

    [Header("Death VFX (spawned when killed by the correct spell)")]
    [Tooltip("Fire enemy -> Snow hit")]
    [SerializeField] private GameObject _fireHitEffect;
    [Tooltip("Water enemy -> Electro hit")]
    [SerializeField] private GameObject _waterHitEffect;
    [Tooltip("Nature enemy -> Green hit (recolor to red to match Fire spell)")]
    [SerializeField] private GameObject _natureHitEffect;
    [Tooltip("Shadow enemy -> Holy hit")]
    [SerializeField] private GameObject _shadowHitEffect;
    [Tooltip("Fallback seconds before the effect is destroyed if it has no ParticleSystem.")]
    [SerializeField] private float _effectLifetime = 3f;

    private SkinnedMeshRenderer _meshRenderer;
    private MaterialPropertyBlock _mpb;
    private WaveManager _waveManager;
    private Animator _animator;

    private bool _tutorialTriggered = false;

    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");

    public ElementType Element => element;
    public EnemyType EnemyType { get; private set; }

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _mpb = new MaterialPropertyBlock();
        _waveManager = FindFirstObjectByType<WaveManager>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        // Reset pooled state
        _tutorialTriggered = false;

        // Re-apply correct material after pooling reuse
        ApplyElementMaterial();
    }

    private void Update()
    {
        if (GameManager.IsPaused)
            return;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (!_tutorialTriggered && transform.position.x < 8f)
        {
            _tutorialTriggered = true;

            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.TryStartTutorial(this);
            }
        }
    }

    public void SetEnemyType(EnemyType type)
    {
        EnemyType = type;
    }

    public void SetElement(ElementType newElement)
    {
        element = newElement;
        ApplyElementMaterial();
    }

    public void SetSpeed(float newSpeed) => speed = newSpeed;

    public void SetHighlight(bool on)
    {
        if (_meshRenderer == null) return;

        _meshRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(OutlineColor, on ? Color.white : Color.black);
        _mpb.SetFloat(OutlineWidth, on ? 0.03f : 0.00f);
        _meshRenderer.SetPropertyBlock(_mpb);
    }

    public void Remove()
    {
        _waveManager?.NotifyEnemyRemoved(gameObject);
    }

    // Killed by the correct spell: play the element's hit VFX, then remove.
    // KillZone escapes still call Remove() directly, so they stay silent.
    public void Die()
    {
        PlayDeathEffect();
        Remove();
    }

    private void PlayDeathEffect()
    {
        GameObject prefab = element switch
        {
            ElementType.Fire => _fireHitEffect,
            ElementType.Water => _waterHitEffect,
            ElementType.Nature => _natureHitEffect,
            ElementType.Shadow => _shadowHitEffect,
            _ => null
        };

        PlayCenteredEffect(prefab);
    }

    // Spawns a one-shot particle effect centered on the enemy's mesh, detached
    // so it survives this enemy being pooled/disabled. scale shrinks/grows it,
    // speed plays it faster (e.g. a quick, small fizzle puff on a wrong cast).
    public void PlayCenteredEffect(GameObject prefab, float scale = 1f, float speed = 1f)
    {
        if (prefab == null) return;

        // Enemy pivot is usually at the feet; use the mesh bounds center.
        Vector3 spawnPos = _meshRenderer != null ? _meshRenderer.bounds.center : transform.position;
        GameObject vfx = Instantiate(prefab, spawnPos, Quaternion.identity);

        bool resize = !Mathf.Approximately(scale, 1f);
        bool respeed = !Mathf.Approximately(speed, 1f);
        if (resize) vfx.transform.localScale = prefab.transform.localScale * scale;

        // Hovl prefabs have looping child systems; force one-shot on every
        // system so it plays once, and size the destroy delay to the longest.
        // simulationSpeed shortens real-time lifetime, so divide by speed.
        float life = _effectLifetime;
        ParticleSystem[] systems = vfx.GetComponentsInChildren<ParticleSystem>();
        if (systems.Length > 0)
        {
            life = 0f;
            foreach (ParticleSystem ps in systems)
            {
                ParticleSystem.MainModule main = ps.main;
                main.loop = false;
                if (resize) main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                if (respeed) main.simulationSpeed *= speed;
                life = Mathf.Max(life, (main.duration + main.startLifetime.constantMax) / Mathf.Max(speed, 0.01f));
            }
        }

        Destroy(vfx, life);
    }

    private void ApplyElementMaterial()
    {
        if (_meshRenderer == null) return;

        _meshRenderer.sharedMaterial = element switch
        {
            ElementType.Fire => _fireMaterial,
            ElementType.Water => _waterMaterial,
            ElementType.Nature => _natureMaterial,
            ElementType.Shadow => _shadowMaterial,
            _ => _fireMaterial
        };
    }

    public void Initialize(ElementType newElement, float newSpeed, Vector3 spawnPosition)
    {
        transform.position = spawnPosition;

        _tutorialTriggered = false;

        SetElement(newElement);
        SetSpeed(newSpeed);

        // Random animation offset so enemies don't sync
        if (_animator != null)
        {
            _animator.Play(0, -1, Random.value);
        }
    }

    public void SetVisualOnly(ElementType newElement)
    {
        SetElement(newElement);

        if (_animator != null)
        {
            _animator.Play(0, -1, Random.value);
        }
    }
}