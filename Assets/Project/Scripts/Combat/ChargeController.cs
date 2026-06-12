using Photon.Pun;
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterInput))]
public class ChargeController : MonoBehaviour, IProvider<Func<bool>>
{
    //
    private EventInput _chargeInput;
    private IStateRequestReceiver _requestReceiver;
    private IManaChecker _manaChecker;

    #region Supporter
    private StateRequester _chargeRequester;
    #endregion

    private void Awake()
    {
        _requestReceiver = GetComponent<IStateRequestReceiver>();
        _manaChecker = GetComponent<IManaChecker>();
        _chargeInput = GetComponent<CharacterInput>()?.ChargeInput;

        _chargeRequester = new StateRequester(StateType.Charge);
    }

    private void OnEnable()
    {
        _chargeInput?.SubscribeInputAction(Charge);
    }

    private void OnDisable()
    {
        _chargeInput?.UnsubscribeInputAction();
    }

    private void Charge()
    {
        if (_manaChecker != null && _manaChecker.CheckFullMana()) return;

        _chargeRequester?.RequestState(_requestReceiver);
    }

    #region Implement IProvider
    public Func<bool> Provide()
        => _chargeInput?.Provide();
    #endregion
}
