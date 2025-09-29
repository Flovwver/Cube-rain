using UnityEngine;
using UnityEngine.Pool;

public class CubesSpawner : MonoBehaviour
{
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private Transform _spawnArea;
    [SerializeField] private int _initialSize = 10;
    [SerializeField] private int _maxSize = 10;
    [SerializeField] private float _repeatRate = 0.5f;

    private ObjectPool<Cube> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Cube>(
            createFunc: CreateCube,
            actionOnGet: OnGetCube,
            actionOnRelease: OnReleaseCube,
            actionOnDestroy: OnDestroyCube,
            collectionCheck: true,
            defaultCapacity: _initialSize,
            maxSize: _maxSize
        );
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetCube), 0f, _repeatRate);
    }

    private void GetCube()
    {
        _pool.Get();
    }

    private Cube CreateCube()
    {
        Cube instance = Instantiate(_cubePrefab);
        instance.gameObject.SetActive(false);
        instance.ReturnToPool = (cube) => _pool.Release(cube);
        return instance;
    }

    private void OnGetCube(Cube cube)
    {
        Vector2 randomDotInCircle = Random.insideUnitCircle * (_spawnArea.localScale.x / 2f);
        Vector3 spawnPoint = new Vector3(randomDotInCircle.x, 0, randomDotInCircle.y) + _spawnArea.position;
        cube.gameObject.SetActive(true);
        cube.Spawn(spawnPoint);
    }

    private void OnReleaseCube(Cube cube)
    {
        cube.ResetForPool();
        cube.gameObject.SetActive(false);
    }

    private void OnDestroyCube(Cube cube)
    {
        if (cube != null)
            Destroy(cube.gameObject);
    }

    private void OnDestroy()
    {
        _pool.Clear();
    }
}
