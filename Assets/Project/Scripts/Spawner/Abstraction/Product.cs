using System;
using UnityEngine;

public abstract class Product : MonoBehaviour
{
    //
    protected int _poolID;
    protected int _productID;
    protected Action<int, int> _onReleaseSelf;

    public int PoolID => _poolID;
    public int ProductID => _productID;

    public abstract void SetContext<T>(T context) where T : ProductContext;

    public virtual void Init(int poolID, int productID, Action<int, int> onRequestRelease)
    {
        _poolID = poolID;
        _productID = productID;
        _onReleaseSelf = onRequestRelease;
    }
}
