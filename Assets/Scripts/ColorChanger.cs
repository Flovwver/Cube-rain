using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private Color _baseColor;

    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void ChangeColorToDefault()
    {
        if (_renderer != null)
        {
            _renderer.material.color = _baseColor;
        }
    }

    public void ChangeColorRandomly()
    {
        if (_renderer != null)
        {
            _renderer.material.color = new Color(Random.value, Random.value, Random.value);
        }
    }
}
