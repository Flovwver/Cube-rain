using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private Color _baseColor;

    private Renderer _renderer;
    private Cube _cube;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _cube = GetComponent<Cube>();
    }

    private void OnEnable()
    {
        _cube.Spawned += OnSpawned;
        _cube.TouchedGroundSurface += OnTouchedGroundSurface;
    }

    private void OnDisable()
    {
        _cube.Spawned -= OnSpawned;
        _cube.TouchedGroundSurface -= OnTouchedGroundSurface;
    }

    private void OnSpawned()
    {
        if (_renderer != null)
        {
            _renderer.material.color = _baseColor;
        }
    }

    private void OnTouchedGroundSurface()
    {
        if (_renderer != null)
        {
            _renderer.material.color = new Color(Random.value, Random.value, Random.value);
        }
    }
}
