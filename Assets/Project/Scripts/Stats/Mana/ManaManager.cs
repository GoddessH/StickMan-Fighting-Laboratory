using Photon.Pun;
using System;
using UnityEngine;

public class ManaManager : MonoBehaviour, IManaRegenerator
{
    //
    [SerializeField] private float _maxMana;
    [SerializeField] private float _regenManaPerHit;

    private PhotonView _photonView;
    private IDamageDealerEvent _damageDealer;
    private float _currentMana = 50;

    public event Action<float, float> OnChangeMana;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _damageDealer = GetComponent<IDamageDealerEvent>();
    }

    private void OnEnable()
    {
        _damageDealer?.UnsubscribeEvent(RegenerateMana);
        _damageDealer?.SubscribeEvent(RegenerateMana);
    }

    private void OnDisable()
    {
        _damageDealer?.UnsubscribeEvent(RegenerateMana);
    }

    [PunRPC]
    private void RPCOnChangeMana(float currentMana, float maxMana)
    {
        OnChangeMana?.Invoke(_currentMana, _maxMana);
    }

    #region Implicit implement IManaRegenerator
    public void RegenerateMana()
    {
        if (_currentMana >= _maxMana) return;

        _currentMana = Mathf.Min(_currentMana + _regenManaPerHit, _maxMana);
        if (_photonView == null) RPCOnChangeMana(_currentMana, _maxMana);
        else _photonView.RPC(nameof(RPCOnChangeMana), RpcTarget.All, _currentMana, _maxMana);
    }
    #endregion
}
