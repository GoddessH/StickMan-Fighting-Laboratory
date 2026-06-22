using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviourPun, ISpawnerSetupHandler
{
    //
    [SerializeField] protected List<ProductEntry> _entryList = new List<ProductEntry>();

    protected IndexedObjectPool[] _pools;

    protected bool _isCollectionCheck = true;
    protected int _defaultCapacity = 10;
    protected int _maxSize = 10000;

    protected virtual void Awake()
        => SetupSpawner();

    protected virtual void OnGetProduct(Product product)
    {
        product.transform.position = transform.position;
        product.gameObject.SetActive(true);
    }
    protected virtual void OnReleaseProduct(Product product)
    {
        product.gameObject.SetActive(false);
    }
    protected virtual void OnDestroyProduct(Product product) => Destroy(product.gameObject);

    protected void HandleReleaseRequest(int poolID, int productID)
    {
        if (poolID < 0 || poolID >= _pools.Length || productID < 0) return;

        _pools[poolID].Release(productID);
    }

    protected virtual void SetupProduct(Product product, int index)
        => product.Init(index, _pools[index].CountAll, HandleReleaseRequest);

    public virtual void Spawn(int poolID = 0,in ProductContext context = null)
    {
        if (poolID < 0 || poolID >= _pools.Length) return;

        Product product = _pools[poolID].GetRandom();
        product.SetContext(context);
    }

    #region Implicit implement ISpawnerSetupHandler
    public void SetupSpawner()
    {
        _pools = new IndexedObjectPool[_entryList.Count];

        for(int i = 0; i < _entryList.Count; ++i)
        {
            int index = i;
            if (_entryList[index].Prefab == null) continue;

            _pools[i] = new IndexedObjectPool(
                () =>
                {
                    Product product = Instantiate(_entryList[index].Prefab, _entryList[index].Holder);
                    SetupProduct(product, index);
                    return product;
                }, OnGetProduct, OnReleaseProduct, OnDestroyProduct, _isCollectionCheck, _defaultCapacity, _maxSize);
        }
    }
    #endregion
}
