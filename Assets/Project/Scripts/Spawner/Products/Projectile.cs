using System;
using System.Collections;
using UnityEngine;

public class Projectile : Product
{
    //
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _speed;

    private RotationHandlerOneTime _rotationHandler;
    private ProjectileContext _context;
    private Coroutine _coroutine;
    private WaitForSeconds _waitForSeconds;

    private void Update()
    {
        if (_context == null) return;

        transform.position += (Vector3)_context.Direction * _speed * Time.deltaTime;
    }

    private void OnEnable()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(LifeRoutine());
    }

    private void OnDisable()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _onReleaseSelf?.Invoke(_poolID, _productID);

        if (!collision.collider.CompareTag(TagNames.HitBoxTag)) return;


        _context?.OnDoDamages.Invoke(collision.collider.GetComponent<HurtPoint>());
        Debug.Log("Called");
    }

    private IEnumerator LifeRoutine()
    {
        if (_waitForSeconds == null) _waitForSeconds = new WaitForSeconds(_lifeTime);
        yield return _waitForSeconds;
        _onReleaseSelf?.Invoke(_poolID, _productID);
    }

    public void SetContext(ProjectileContext context)
    {
        _context = context;

        transform.position = _context.SpawnPosition;

        if (_rotationHandler == null) _rotationHandler = GetComponent<RotationHandlerOneTime>();
        _rotationHandler.Rotate(_context.Direction);
    }
}
