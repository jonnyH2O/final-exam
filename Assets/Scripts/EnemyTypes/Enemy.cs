using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private ElementType element;
    [SerializeField] private float speed = 2f;

    [SerializeField] private Material _fireMaterial;
    [SerializeField] private Material _waterMaterial;
    [SerializeField] private Material _natureMaterial;
    [SerializeField] private Material _shadowMaterial;

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
}