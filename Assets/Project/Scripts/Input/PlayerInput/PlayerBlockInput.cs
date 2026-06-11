using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBlockInput : EventInput
{
    //
    private PhotonView _ownerPhotonView;
    private InputAction _blockAction;

    public PlayerBlockInput(PhotonView ownerPhotonView)
    {
        _ownerPhotonView = ownerPhotonView;
        _blockAction = InputSystem.actions.FindAction("Block");
    }

    private void OnBlock(InputAction.CallbackContext ctx)
    {
        if (_ownerPhotonView != null && !_ownerPhotonView.IsMine) return;
        //Debug.Log("Block Input Detected");
        _cachedSubscriber?.Invoke();
    }

    private bool OnCheckInput()
    {
        if (_ownerPhotonView != null && !_ownerPhotonView.IsMine) return false;

        if (_blockAction == null) return false;
        return _blockAction.IsPressed();
    }

    #region Implement BlockInput
    public override void SubscribeInputAction(Action subscriber)
    {
        if (_blockAction == null) return;
        _cachedSubscriber = subscriber;
        _blockAction.started -= OnBlock;
        _blockAction.started += OnBlock;
    }

    public override void UnsubscribeInputAction()
    {
        if (_blockAction == null) return;
        _blockAction.started -= OnBlock;
    }

    public override Func<bool> Provide()
        => OnCheckInput;
    #endregion
}
