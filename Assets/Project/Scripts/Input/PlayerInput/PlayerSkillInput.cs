using Photon.Pun;
using System;
using UnityEngine.InputSystem;

public class PlayerSkillInput : EventInput
{
    private PhotonView _ownerPhotonView;
    private InputAction _skillAction;

    public PlayerSkillInput(PhotonView ownerPhotonView, string actionName)
    {
        _ownerPhotonView = ownerPhotonView;
        _skillAction = InputSystem.actions.FindAction(actionName);
    }

    private void OnSkill(InputAction.CallbackContext ctx)
    {
        if (_ownerPhotonView != null && !_ownerPhotonView.IsMine) return;
        _cachedSubscriber?.Invoke();
    }

    private bool OnCheckInput()
    {
        if (_ownerPhotonView != null && !_ownerPhotonView.IsMine) return false;
        if (_skillAction == null) return false;
        return _skillAction.WasPressedThisFrame();
    }

    #region Implement EventInput
    public override void SubscribeInputAction(Action subscriber)
    {
        if (_skillAction == null) return;
        _cachedSubscriber = subscriber;
        _skillAction.started -= OnSkill;
        _skillAction.started += OnSkill;
    }

    public override void UnsubscribeInputAction()
    {
        if (_skillAction == null) return;
        _skillAction.started -= OnSkill;
    }

    public override Func<bool> Provide()
        => OnCheckInput;
    #endregion
}
