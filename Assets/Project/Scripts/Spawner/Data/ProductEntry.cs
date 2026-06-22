using System;
using UnityEngine;

[Serializable]
public struct ProductEntry
{
    //
    [SerializeField] private Product _productPrefab;
    [SerializeField] private Transform _productHolder;

    public Product Prefab => _productPrefab;
    public Transform Holder => _productHolder;
}
