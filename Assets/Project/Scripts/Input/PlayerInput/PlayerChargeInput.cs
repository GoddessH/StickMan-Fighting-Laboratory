using Photon.Pun;
using System;
using UnityEngine.InputSystem;

public class PlayerChargeInput : EventInput
{
    // 
    private PhotonView _ownerPhotonView;
    private InputAction _chargeAction;

    public PlayerChargeInput(PhotonView photonView)
    {
        _ownerPhotonView = photonView;
        _chargeAction = InputSystem.actions.FindAction(InputActionName.ChargeAction);
    }

    private void Charge(InputAction.CallbackContext ctx)
    {
        if (_ownerPhotonView != null && !_ownerPhotonView.IsMine) return;

        _cachedSubscriber?.Invoke();
    }

    private bool CheckHoldingInput()
    {
        if (_ownerPhotonView != null && !_ownerPhotonView.IsMine) return false;
        if (_chargeAction == null) return false;

        return _chargeAction.IsPressed();
    }

    #region Implement EventInput
    public override void SubscribeInputAction(Action subscriber)
    {
        if (_chargeAction == null) return;

        _cachedSubscriber = subscriber;
        _chargeAction.started -= Charge;
        _chargeAction.started += Charge;
    }

    public override void UnsubscribeInputAction()
    {
        if (_chargeAction == null) return;

        _chargeAction.started -= Charge;
    }

    public override Func<bool> Provide()
        => CheckHoldingInput;
    #endregion
}
