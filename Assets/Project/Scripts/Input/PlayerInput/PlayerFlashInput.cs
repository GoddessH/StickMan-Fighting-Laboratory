using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlashInput : FlashInput
{
    private PhotonView _ownerPhotonView;
    private InputAction _flashAction;

    public PlayerFlashInput(PhotonView ownerPhotonView)
    {
        _ownerPhotonView = ownerPhotonView;
        _flashAction = InputSystem.actions.FindAction("Sprint"); // Sprint is Shift key by default in Actions asset
    }

    private void OnFlash(InputAction.CallbackContext ctx)
    {
        if (_ownerPhotonView != null && !_ownerPhotonView.IsMine) return;
        _cachedSubscriber?.Invoke();
    }

    private bool OnCheckInput()
    {
        if (_ownerPhotonView != null && !_ownerPhotonView.IsMine) return false;
        if (_flashAction == null) return false;
        return _flashAction.IsPressed();
    }

    #region Implement FlashInput
    public override void SubscribeFlashAction(Action subscriber)
    {
        if (_flashAction == null) return;
        _cachedSubscriber = subscriber;
        _flashAction.started -= OnFlash;
        _flashAction.started += OnFlash;
    }

    public override void UnsubscribeFlashAction()
    {
        if (_flashAction == null) return;
        _flashAction.started -= OnFlash;
    }

    public override Func<bool> Provide()
        => OnCheckInput;
    #endregion
}
