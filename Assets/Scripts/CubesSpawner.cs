using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class CubesSpawner : MonoBehaviour
{
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private Transform _spawnArea;
    [SerializeField] private int _initialSize = 10;
    [SerializeField] private int _maxSize = 10;
    [SerializeField] private float _repeatRate = 0.5f;
    [SerializeField] private bool _spawning = true;

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
        StartCoroutine(SpawnLoop());
    }

    private void OnDestroy()
    {
        _pool.Clear();
    }

    private IEnumerator SpawnLoop()
    {
        while (_spawning)
        {
            yield return new WaitForSeconds(_repeatRate);
            SpawnCube();
        }
    }

    private void SpawnCube()
    {
        _pool.Get();
    }

    private Cube CreateCube()
    {
        Cube instance = Instantiate(_cubePrefab);
        instance.gameObject.SetActive(false);

        instance.ReturnedToPool += _pool.Release;
        return instance;
    }

    private void OnGetCube(Cube cube)
    {
        Vector2 randomDotInCircle = Random.insideUnitCircle * (_spawnArea.localScale.x / 2f);
        Vector3 spawnPoint = new Vector3(randomDotInCircle.x, 0, randomDotInCircle.y) + _spawnArea.position;
        cube.Spawn(spawnPoint);
        cube.gameObject.SetActive(true);
    }

    private void OnReleaseCube(Cube cube)
    {
        cube.ResetForPool();
        cube.gameObject.SetActive(false);
    }

    private void OnDestroyCube(Cube cube)
    {
        if (cube != null)
        {
            cube.ReturnedToPool -= _pool.Release;
            Destroy(cube.gameObject);
        }
    }
}
