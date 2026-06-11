using System;
using UnityEngine;

public class HurtController : MonoBehaviour, IProvider<Func<float>>, IDamageTakerEvent
{
    //
    [SerializeField] private HurtPoint _hurtPoint;

    private Action _onTakeDamage;
    private IStateRequestReceiver _requestReceiver;
    private float _cachedLastDamages;

    private BlockController _blockController;

    #region Supporter
    private StateRequester _hurtRequester;
    #endregion

    private void Awake()
    {
        _requestReceiver = GetComponent<IStateRequestReceiver>();
        _hurtRequester = new StateRequester(StateType.Hurt);
        _hurtPoint?.Init(Hurt);
        _blockController = GetComponent<BlockController>();
    }

    private void Hurt(float damages)
    {
        if (_blockController != null && _blockController.IsBlocking)
        {
            _blockController.SpawnBlockFlashVFX();
            return;
        }

        _cachedLastDamages = damages;
        _hurtRequester?.RequestState(_requestReceiver);
        _onTakeDamage?.Invoke();
    }

    #region Implement IProvider
    /// <summary>
    /// Provide cachedLastDamages getter
    /// </summary>
    public Func<float> Provide()
        => () => _cachedLastDamages;
    #endregion

    #region Explicit implement IDamageTakerEvent
    void IDamageTakerEvent.SubscribeTakeDamageEvent(Action subscriber)
    {
        _onTakeDamage -= subscriber;
        _onTakeDamage += subscriber;
    }
    void IDamageTakerEvent.UnsubscribeTakeDamageEvent(Action unSubscriber)
    {
        _onTakeDamage -= unSubscriber;
    }
    #endregion
}
