using Photon.Pun;
using System;
using UnityEngine.InputSystem;

public class PlayerAttackInput : EventInput
{
    //
    private PhotonView _ownerPhotonView;
    private InputAction _attackAction;

    public PlayerAttackInput(PhotonView ownerPhotonView)
    {
        _ownerPhotonView = ownerPhotonView;
        _attackAction = InputSystem.actions.FindAction(InputActionName.AttackAction);
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (_ownerPhotonView != null && !_ownerPhotonView.IsMine) return;

        _cachedSubscriber?.Invoke();
    }

    private bool OnCheckInput()
    {
        if (_ownerPhotonView != null && !_ownerPhotonView.IsMine) return false;

        if (_attackAction == null) return false;
        return _attackAction.WasPressedThisFrame();
    }

    #region Implement AttackInput
    public override void SubscribeInputAction(Action subscriber)
    {
        if (_attackAction == null) return;
        _cachedSubscriber = subscriber;
        _attackAction.started -= OnAttack;
        _attackAction.started += OnAttack;
    }

    public override void UnsubscribeInputAction()
    {
        if (_attackAction == null) return;
        _attackAction.started -= OnAttack;
    }

    public override Func<bool> Provide()
        => OnCheckInput;
    #endregion
}
