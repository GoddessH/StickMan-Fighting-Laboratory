using Photon.Pun;
using System;
using UnityEngine;

public class ManaManager : MonoBehaviour, IManaRegenerator, IManaChecker
{
    //
    [SerializeField] private float _maxMana;
    [SerializeField] private float _regenManaPerHit;

    private PhotonView _photonView;
    private IDamageTakerEvent _damageTaker;
    private float _currentMana = 50;

    public event Action<float, float> OnChangeMana;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _damageTaker = GetComponent<IDamageTakerEvent>();
    }

    private void Start()
    {
        OnChangeMana?.Invoke(_currentMana, _maxMana);
    }

    private void OnEnable()
    {
        _damageTaker?.SubscribeTakeDamageEvent(((IManaRegenerator)this).RegenerateMana);
    }

    private void OnDisable()
    {
        _damageTaker?.UnsubscribeTakeDamageEvent(((IManaRegenerator)this).RegenerateMana);
    }

    [PunRPC]
    private void RPCOnChangeMana(float currentMana, float maxMana)
    {
        OnChangeMana?.Invoke(_currentMana, _maxMana);
    }

    #region Explicit implement IManaRegenerator
    void IManaRegenerator.RegenerateMana()
    {
        Debug.Log("Phase1");
        if (_currentMana >= _maxMana) return;
        _currentMana = Mathf.Min(_currentMana + _regenManaPerHit, _maxMana);
        if (_photonView == null) RPCOnChangeMana(_currentMana, _maxMana);
        else _photonView.RPC(nameof(RPCOnChangeMana), RpcTarget.All, _currentMana, _maxMana);
        Debug.Log("Phase2");
    }
    #endregion

    #region Explicit implement IManaChecker
    bool IManaChecker.CheckFullMana()
        => _currentMana >= _maxMana;
    #endregion
}
