using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Cube : MonoBehaviour
{
    [SerializeField] private float _minLifetimeAfterContact = 2f;
    [SerializeField] private float _maxLifetimeAfterContact = 5f;
    [SerializeField] private ColorChanger _colorChanger;

    private bool _hasLanded = false;
    private Coroutine _lifetimeRoutine;
    private Rigidbody _rigidbody;

    public event Action<Cube> ReturnedToPool;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasLanded)
            return;

        if (collision.collider.GetComponentInParent<GroundSurface>() != null)
        {
            _hasLanded = true;

            _colorChanger.ChangeColorRandomly();

            float lifetimeAfterContact = UnityEngine.Random.Range(_minLifetimeAfterContact, _maxLifetimeAfterContact);
            StartLifetimeRoutine(lifetimeAfterContact);
        }
    }

    public void Spawn(Vector3 spawnPoint)
    {
        transform.position = spawnPoint;
        _hasLanded = false;
        StopLifetimeRoutineIfAny();
        ResetPhysicsState();
        _colorChanger.ChangeColorToDefault();
    }

    public void ResetForPool()
    {
        StopLifetimeRoutineIfAny();
        _hasLanded = false;
        ResetPhysicsState();
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
        ReturnedToPool?.Invoke(this);
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
