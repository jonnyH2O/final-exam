using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private ElementType element;
    [SerializeField] private float speed = 2f;

    private Renderer _rend; // Cache the renderer for color changes
    private WaveManager _waveManager; // Reference to the WaveManager to notify when removed
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor"); 
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    public ElementType Element => element; 
    public EnemyType EnemyType { get; private set; }

    private void Awake()
    {
        _rend = GetComponent<Renderer>(); 
        _waveManager = FindFirstObjectByType<WaveManager>(); 
    }

    private void Start()
    {
        ApplyElementColor();
    }

    private void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    public void SetElement(ElementType newElement)
    {
        element = newElement;
        ApplyElementColor();
    }

    public void SetSpeed(float newSpeed) => speed = newSpeed;

    public void SetEnemyType(EnemyType type) => EnemyType = type;

    public void SetHighlight(bool on)
    {
        _rend.material.EnableKeyword("_EMISSION");
        _rend.material.SetColor(EmissionColor, on ? Color.yellow * 2f : Color.black);
    }

    public void Remove() => _waveManager.NotifyEnemyRemoved(gameObject); // Return to pool, not destroy

    private void ApplyElementColor()
    {
        Color c = element switch
        {
            ElementType.Fire   => new Color(1f, 0.25f, 0f),
            ElementType.Water  => new Color(0f, 0.5f, 1f),
            ElementType.Nature => new Color(0.1f, 0.8f, 0.1f),
            ElementType.Shadow => new Color(0.3f, 0f, 0.5f),
            _                  => Color.white
        };
        _rend.material.SetColor(BaseColor, c);
    }
}