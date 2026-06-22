using Photon.Pun;
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

    protected void HandleReleaseRequest(int poolID, int productID)
    {

    }

    protected void OnGetProduct(Product product) => product.gameObject.SetActive(true);
    protected void OnReleaseProduct(Product product) => product.gameObject.SetActive(false);
    protected void OnDestroyProduct(Product product) => Destroy(product.gameObject);

    public virtual void Spawn(int poolID) => _pools[poolID].GetRandom();

    #region Implicit implement ISpawnerSetupHandler
    public void SetupSpawner()
    {
        _pools = new IndexedObjectPool[_entryList.Count];

        for(int i = 0; i < _entryList.Count; ++i)
        {
            if (_entryList[i].Prefab == null) continue;

            _pools[i] = new IndexedObjectPool(
                () =>
                {
                    int index = i;
                    Product product = Instantiate(_entryList[index].Prefab, _entryList[index].Holder);
                    product.Init(index, _pools[index].CountAll, HandleReleaseRequest);
                    return product;
                }, OnGetProduct, OnReleaseProduct, OnDestroyProduct, _isCollectionCheck, _defaultCapacity, _maxSize);
        }
    }
    #endregion
}
