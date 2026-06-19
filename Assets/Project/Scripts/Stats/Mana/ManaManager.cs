using Photon.Pun;
using System;
using UnityEngine;

public class ManaManager : MonoBehaviour, IManaRegenerator, IManaConsumer, IManaChecker
{
    //
    [SerializeField] private float _maxMana;
    [SerializeField] private float _regenManaPerHit;

    private PhotonView _photonView;
    private IDamageTakerEvent _damageTaker;

    private Action<float, float> _onChangeMana;

    private float _currentMana = 50;

    public event Action<float, float> OnChangeMana
    {
        add
        {
            _onChangeMana -= value;
            _onChangeMana += value;
            _onChangeMana?.Invoke(_currentMana, _maxMana);
        }
        remove => _onChangeMana -= value;
    }

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _damageTaker = GetComponent<IDamageTakerEvent>();
    }

    private void OnEnable() => _damageTaker?.SubscribeTakeDamageEvent(((IManaRegenerator)this).RegenerateMana);

    private void OnDisable() => _damageTaker?.UnsubscribeTakeDamageEvent(((IManaRegenerator)this).RegenerateMana);

    [PunRPC]
    private void RPCOnChangeMana(float currentMana, float maxMana)
    {
        _currentMana = currentMana;
        _maxMana = maxMana;
        _onChangeMana?.Invoke(_currentMana, _maxMana);
    }

    private void ChangeManaCallBack()
    {
        if (_photonView == null) RPCOnChangeMana(_currentMana, _maxMana);
        else _photonView.RPC(nameof(RPCOnChangeMana), RpcTarget.All, _currentMana, _maxMana);
    }

    #region Implicit implement IManaRegenerator
    void IManaRegenerator.RegenerateMana()
    {
        if (_currentMana >= _maxMana) return;

        _currentMana = Mathf.Min(_currentMana + _regenManaPerHit, _maxMana);
        ChangeManaCallBack();
    }
    #endregion

    #region Explicit implement IManaConsumer
    void IManaConsumer.ConsumeMana(float amount)
    {
        _currentMana = Mathf.Max(0f, _currentMana - amount);

        ChangeManaCallBack();
    }
    #endregion

    #region Explicit implement IManaChecker
    bool IManaChecker.IsFullMana() => _currentMana >= _maxMana;

    bool IManaChecker.HasManaReached(float amount) => _currentMana >= amount;

    float IManaChecker.GetCurrentMana() => _currentMana;

    float IManaChecker.GetMaxMana() => _maxMana;
    #endregion
}
