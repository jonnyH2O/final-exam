using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Inspector Fields 
    [SerializeField] private ElementType element;
    [SerializeField] private float speed = 2f;

    // Private Fields 
    private Renderer rend;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    // Public Properties 
    public ElementType Element => element;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        // Temp visual to differentiate enemy types, we can replace this with proper art later
        rend.material.color = element switch
        {
            ElementType.Fire    => new Color(1f, 0.25f, 0f),
            ElementType.Water   => new Color(0f, 0.5f, 1f),
            ElementType.Nature  => new Color(0.1f, 0.8f, 0.1f),
            ElementType.Shadow  => new Color(0.3f, 0f, 0.5f),
            _ => Color.white
        };
    }
    private void Update()
    {
        // Move enemy across the screen from right to left
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    // Public Methods 
    public void SetHighlight(bool on)
    {
        // Toggles highlight by enabling/disabling emission on the material
        // This is a temporary visual effect to show target, we can replace it with something more polished later
        if (on)
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor(EmissionColor, Color.yellow * 2f);
        }
        else
        {
            rend.material.DisableKeyword("_EMISSION");
        }
    }
}
