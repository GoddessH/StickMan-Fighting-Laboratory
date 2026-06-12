using UnityEngine;

public class VFXSpawner : SingletonPun<VFXSpawner>
{
    //
    [SerializeField] private GameObject _chargeAura;
    private GameObject _chargeInstance;

    public void SpawnChargeAura(Transform parentTransform = null, bool isDestroy = false)
    {
        if (isDestroy && _chargeInstance != null)
        {
            Destroy(_chargeInstance);
            return;
        }

        if (parentTransform == null) _chargeInstance = Instantiate(_chargeAura);
        else _chargeInstance = Instantiate(_chargeAura, parentTransform);
        _chargeAura.transform.position = Vector3.zero;
    }
}
