using System;
using System.Collections.Generic;
using UnityEngine.Pool;

//Serving only Product
public class IndexedObjectPool<T> where T: Product
{
    //
    private ObjectPool<T> _pool;
    private Dictionary<int, T> _dictionary = new Dictionary<int, T>();

    public int CountAll => _pool.CountAll;
    public int CountActive => _pool.CountActive;
    public int CountInactive => _pool.CountInactive;

    public IndexedObjectPool(Func<T> createFunc, Action<T> onGet, Action<T> onRelease = null, Action<T> onDestroy = null, 
        bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        => _pool = new ObjectPool<T>(createFunc, onGet, onRelease, onDestroy, collectionCheck, defaultCapacity, maxSize);


    public T GetRandom()
    {
        T product = _pool.Get();

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
