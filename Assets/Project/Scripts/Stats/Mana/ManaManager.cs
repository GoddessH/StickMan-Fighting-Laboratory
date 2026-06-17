using Photon.Pun;
using System;
using UnityEngine;

public class ManaManager : MonoBehaviour, IManaRegenerator, IManaConsumer, IManaState
{
    //
    [SerializeField] private float _maxMana;
    [SerializeField] private float _regenManaPerHit;

    private PhotonView _photonView;
    private IDamageTakerEvent _damageTaker;
    private float _currentMana = 50;

    public event Action<float, float> OnChangeMana;

    public float GetCurrentMana()
    {
        return _currentMana;
    }

    public bool HasEnoughMana(float amount)
    {
        return _currentMana >= amount;
    }

    public void ConsumeMana(float amount)
    {
        _currentMana = Mathf.Max(0f, _currentMana - amount);
        
        Debug.Log($"[ManaManager] Deducted: -{amount:F0} Mana | Current Mana: {_currentMana:F0}/{_maxMana:F0}");

        if (_photonView == null) RPCOnChangeMana(_currentMana, _maxMana);
        else _photonView.RPC(nameof(RPCOnChangeMana), RpcTarget.All, _currentMana, _maxMana);
    }

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _damageTaker = GetComponent<IDamageTakerEvent>();
    }

    private void OnEnable()
    {
        _damageTaker?.SubscribeTakeDamageEvent(RegenerateMana);
    }

    private void OnDisable()
    {
        _damageTaker?.UnsubscribeTakeDamageEvent(RegenerateMana);
    }

    [PunRPC]
    private void RPCOnChangeMana(float currentMana, float maxMana)
    {
        _currentMana = currentMana;
        _maxMana = maxMana;
        OnChangeMana?.Invoke(_currentMana, _maxMana);
    }

    #region Implicit implement IManaRegenerator
    public void RegenerateMana()
    {
        if (_currentMana >= _maxMana) return;

        _currentMana = Mathf.Min(_currentMana + _regenManaPerHit, _maxMana);

        Debug.Log($"[ManaManager] Regenerated: +{_regenManaPerHit:F0} Mana (Hit) | Current Mana: {_currentMana:F0}/{_maxMana:F0}");

        if (_photonView == null) RPCOnChangeMana(_currentMana, _maxMana);
        else _photonView.RPC(nameof(RPCOnChangeMana), RpcTarget.All, _currentMana, _maxMana);
    }
    #endregion
}
