using Photon.Pun;
using UnityEngine;

public class VFXSpawner : MonoBehaviourPun
{
    //
    [SerializeField] private GameObject _chargeAura;
    private GameObject _chargeInstance;

    [PunRPC]
    private void RPCSpawnChargeAura(int viewID)
    {
        Transform ownerTransform = PhotonView.Find(viewID)?.transform;

        if (ownerTransform == null) return;

        _chargeInstance = Instantiate(_chargeAura, ownerTransform);
        _chargeAura.transform.position = Vector3.zero;
    }

    [PunRPC]
    private void RPCDestroyChargeAura()
    {
        if (_chargeInstance == null) return;
        Destroy(_chargeInstance);
    }

    public void SpawnChargeAura(int viewID)
    {
        photonView.RPC(nameof(RPCSpawnChargeAura), RpcTarget.All, viewID);
    }

    public void DestroyChargeAura()
    {
        photonView.RPC(nameof(RPCDestroyChargeAura), RpcTarget.All);
    }
}
