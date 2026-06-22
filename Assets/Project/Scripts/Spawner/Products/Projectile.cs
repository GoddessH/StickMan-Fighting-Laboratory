using System;
using System.Collections;
using UnityEngine;

public class Projectile : Product
{
    //
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _speed;

    private RotationHandlerOneTime _rotationHandler;
    private ProductContext _productContext;
    private Coroutine _coroutine;
    private WaitForSeconds _waitForSeconds;

    private void Update()
    {
        if (_productContext == null) return;

        transform.position += (Vector3)_productContext.Direction * _speed * Time.deltaTime;
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
        if (!collision.collider.CompareTag(TagNames.HitBoxTag)) return;

        _productContext?.OnDoDamages.Invoke(collision.collider.GetComponent<HurtPoint>());
        _onReleaseSelf?.Invoke(_poolID, _productID);
    }

    private IEnumerator LifeRoutine()
    {
        if (_waitForSeconds == null) _waitForSeconds = new WaitForSeconds(_lifeTime);
        yield return _waitForSeconds;
        _onReleaseSelf?.Invoke(_poolID, _productID);
    }

    #region Implement Product
    public override void SetContext<ProductContext>(ProductContext context)
    {
        _productContext = context;

        if (_rotationHandler == null) _rotationHandler = GetComponent<RotationHandlerOneTime>();
        _rotationHandler.Rotate(_productContext.Direction);
    }
    #endregion
}
