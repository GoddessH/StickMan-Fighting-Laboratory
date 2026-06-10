using System;
using UnityEngine;

public class HurtController : MonoBehaviour, IProvider<Func<float>>
{
    //
    [SerializeField] private HurtPoint _hurtPoint;

    private IStateRequestReceiver _requestReceiver;
    private float _cachedLastDamages;

    #region Supporter
    private StateRequester _hurtRequester;
    #endregion

    private void Awake()
    {
        _requestReceiver = GetComponent<IStateRequestReceiver>();
        _hurtRequester = new StateRequester(StateType.Hurt);
        _hurtPoint?.Init(Hurt);
    }

    private void Hurt(float damages)
    {
        _cachedLastDamages = damages;
        _hurtRequester?.RequestState(_requestReceiver);
    }

    #region Implement IProvider
    /// <summary>
    /// Provide cachedLastDamages getter
    /// </summary>
    public Func<float> Provide()
        => () => _cachedLastDamages;
    #endregion
}
