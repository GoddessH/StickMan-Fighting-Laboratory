using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner<T> : MonoBehaviourPun, ISpawnerSetupHandler where T : Product
{
    //
    [SerializeField] protected List<ProductEntry<T>> _entryList = new List<ProductEntry<T>>();

    protected IndexedObjectPool<T>[] _pools;

    protected bool _isCollectionCheck = true;
    protected int _defaultCapacity = 10;
    protected int _maxSize = 10000;

    protected virtual void Awake()
        => SetupSpawner();

    protected virtual void OnGetProduct(T product)
    {
        product.transform.position = transform.position;
        product.gameObject.SetActive(true);
    }
    protected virtual void OnReleaseProduct(T product)
    {
        product.gameObject.SetActive(false);
    }
    protected virtual void OnDestroyProduct(T product) => Destroy(product.gameObject);

    protected void HandleReleaseRequest(int poolID, int productID)
    {
        if (poolID < 0 || poolID >= _pools.Length || productID < 0) return;

        _pools[poolID].Release(productID);
    }

    protected virtual void SetupProduct(T product, int index)
        => product.Init(index, _pools[index].CountAll, HandleReleaseRequest);

    #region Implicit implement ISpawnerSetupHandler
    public void SetupSpawner()
    {
        _pools = new IndexedObjectPool<T>[_entryList.Count];

        for(int i = 0; i < _entryList.Count; ++i)
        {
            int index = i;
            if (_entryList[index].Prefab == null) continue;

            _pools[i] = new IndexedObjectPool<T>(
                () =>
                {
                    T product = Instantiate(_entryList[index].Prefab, _entryList[index].Holder);
                    SetupProduct(product, index);
                    return product;
                }, OnGetProduct, OnReleaseProduct, OnDestroyProduct, _isCollectionCheck, _defaultCapacity, _maxSize);
        }
    }
    #endregion
}
