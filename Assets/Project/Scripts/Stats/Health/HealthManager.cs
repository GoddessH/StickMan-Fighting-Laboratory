using System;
using UnityEngine;

public class HealthManager : MonoBehaviour, IProvider<Action<float>>
{
    // 
    [SerializeField] private float _maxHealth;

    private Action _onLoseAllHealth;

    public float CurrentHealth { get; private set; }

    /// <summary>
    /// Callback whenever health is increased or decreased
    /// </summary>
    public event Action<float, float> OnChangeHealth;


    private void Awake()
    {
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
        OnChangeHealth?.Invoke(CurrentHealth, _maxHealth);

        if (CurrentHealth <= 0) _onLoseAllHealth?.Invoke();
    }

    #region Implement IProvider
    /// <summary>
    /// Provide HurtState
    /// </summary>
    public Action<float> Provide()
        => ReduceHealth;
    #endregion
}
