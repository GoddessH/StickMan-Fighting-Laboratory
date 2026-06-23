using System;
using UnityEngine;

[Serializable]
public struct ProductEntry<T> where T : Product
{
    //
    [SerializeField] private T _productPrefab;
    [SerializeField] private Transform _productHolder;

    public T Prefab => _productPrefab;
    public Transform Holder => _productHolder;
}
