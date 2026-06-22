using System;
using System.Collections.Generic;
using UnityEngine.Pool;

//Serving only Product
public class IndexedObjectPool
{
    //
    private ObjectPool<Product> _pool;
    private Dictionary<int, Product> _dictionary = new Dictionary<int, Product>();

    public int CountAll => _pool.CountAll;
    public int CountActive => _pool.CountActive;
    public int CountInactive => _pool.CountInactive;

    public IndexedObjectPool(Func<Product> createFunc, Action<Product> onGet, Action<Product> onRelease = null, Action<Product> onDestroy = null, 
        bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        => _pool = new ObjectPool<Product>(createFunc, onGet, onRelease, onDestroy, collectionCheck, defaultCapacity, maxSize);


    public Product GetRandom()
    {
        Product product = _pool.Get();

        if (_dictionary.ContainsKey(product.ProductID)) return product;

        _dictionary.Add(product.ProductID, product);
        return product;
    }

    public Product GetExistedAndActive(int id)
    {
        if (!_dictionary.ContainsKey(id) || !_dictionary[id].gameObject.activeSelf) return null;
        return _dictionary[id];
    }

    public void Release(int id)
    {
        if (!_dictionary.ContainsKey(id) || !_dictionary[id].gameObject.activeSelf) return;
        _pool.Release(_dictionary[id]);
    }
}
