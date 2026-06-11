using Photon.Pun;
using System;
using UnityEngine;

public class HealthManager : MonoBehaviour, IProvider<Action<float>>
{
    // 
    [SerializeField] private float _maxHealth;

    private PhotonView _photonView;
    private Action _onLoseAllHealth;

    public float CurrentHealth { get; private set; }

    /// <summary>
    /// Callback whenever health is increased or decreased
    /// </summary>
    public event Action<float, float> OnChangeHealth;


    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        DeadController deadController = GetComponent<DeadController>();
        if (deadController != null) _onLoseAllHealth = deadController.RequestDeadState;

        CurrentHealth = _maxHealth;
    }

    private void IncreaseHealth(float amount)
    {
        if (CurrentHealth >= _maxHealth) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, _maxHealth);
        OnChangeHealth?.Invoke(CurrentHealth, _maxHealth);
    }

    private void ReduceHealth(float amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);

        if (_photonView == null) RPCOnChangeHeatlh(CurrentHealth, _maxHealth);
        else _photonView.RPC(nameof(RPCOnChangeHeatlh), RpcTarget.All, CurrentHealth, _maxHealth);

        if (CurrentHealth <= 0) _onLoseAllHealth?.Invoke();
    }

    [PunRPC]
    private void RPCOnChangeHeatlh(float currentHealth, float maxHealth)
    {
        OnChangeHealth?.Invoke(currentHealth, maxHealth);
    }

    #region Implement IProvider
    /// <summary>
    /// Provide HurtState
    /// </summary>
    public Action<float> Provide()
        => ReduceHealth;
    #endregion
}
