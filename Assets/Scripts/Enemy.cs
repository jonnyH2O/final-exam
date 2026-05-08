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
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");

    public ElementType Element => element;
    public EnemyType EnemyType { get; private set; }

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _mpb = new MaterialPropertyBlock();
        _waveManager = FindFirstObjectByType<WaveManager>();
    }

    private void Start()
    {
        ApplyElementMaterial();
    }

    private void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    public void SetElement(ElementType newElement)
    {
        element = newElement;
        ApplyElementMaterial();
    }

    public void SetSpeed(float newSpeed) => speed = newSpeed;

    public void SetEnemyType(EnemyType type) => EnemyType = type;

    public void SetHighlight(bool on)
    {
        _meshRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(OutlineColor, on ? Color.white : Color.black);
        _mpb.SetFloat(OutlineWidth, on ? 0.03f : 0.00f);
        _meshRenderer.SetPropertyBlock(_mpb);
    }
    public void Remove() => _waveManager.NotifyEnemyRemoved(gameObject);

    private void ApplyElementMaterial()
    {
        // Swap the whole material asset — sharedMaterial avoids creating an instance
        _meshRenderer.sharedMaterial = element switch
        {
            ElementType.Fire   => _fireMaterial,
            ElementType.Water  => _waterMaterial,
            ElementType.Nature => _natureMaterial,
            ElementType.Shadow => _shadowMaterial,
            _                  => _fireMaterial
        };
    }

    public void Initialize(ElementType newElement, float newSpeed, Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        SetElement(newElement);
        SetSpeed(newSpeed);

        // Random offset so enemies don't animate in sync
        Animator anim = GetComponentInChildren<Animator>();
        anim.Play(0, -1, Random.value);
    }

}