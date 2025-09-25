using System;
using System.Collections;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private float _minLifetimeAfterContact = 2f;
    [SerializeField] private float _maxLifetimeAfterContact = 5f;
    [SerializeField] private Color _baseColor = Color.white;

    private bool _hasLanded = false;
    private Coroutine _lifetimeRoutine;
    private Renderer _renderer;
    private Rigidbody _rigidbody;

    public Action<Cube> ReturnToPool;

    public void Spawn(Vector3 spawnPoint)
    {
        transform.position = spawnPoint;
        gameObject.SetActive(true);
        _hasLanded = false;
        StopLifetimeRoutineIfAny();
        ResetPhysicsState();
    }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasLanded) 
            return;

        if (collision.collider.GetComponentInParent<GroundSurface>() != null)
        {
            _hasLanded = true;
            ChangeColorRandomly();

            float lifetimeAfterContact = UnityEngine.Random.Range(_minLifetimeAfterContact, _maxLifetimeAfterContact);
            StartLifetimeRoutine(lifetimeAfterContact);
        }
    }

    private void ChangeColorRandomly()
    {
        if (_renderer != null)
        {
            _renderer.material.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }
    }

    private void StartLifetimeRoutine(float lifetime)
    {
        StopLifetimeRoutineIfAny();
        _lifetimeRoutine = StartCoroutine(LifetimeCoroutine(lifetime));
    }

    private void StopLifetimeRoutineIfAny()
    {
        if (_lifetimeRoutine != null)
        {
            StopCoroutine(_lifetimeRoutine);
            _lifetimeRoutine = null;
        }
    }

    private IEnumerator LifetimeCoroutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        ReturnToPool?.Invoke(this);
    }

    public void ResetForPool()
    {
        StopLifetimeRoutineIfAny();
        _hasLanded = false;
        ChangeColorToDefault();
        ResetPhysicsState();
        gameObject.SetActive(false);
    }

    private void ChangeColorToDefault()
    {
        if (_renderer != null)
        {
            _renderer.material.color = _baseColor;
        }
    }

    private void ResetPhysicsState()
    {
        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = false;
        }
    }
}
