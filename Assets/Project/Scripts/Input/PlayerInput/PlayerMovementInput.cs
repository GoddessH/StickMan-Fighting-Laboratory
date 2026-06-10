using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementInput : MovementInput
{
    //
    private PhotonView _ownerPhotonView;
    private InputAction _movementAction;

    public PlayerMovementInput(PhotonView photonView)
    {
        _ownerPhotonView = photonView;
        _movementAction = InputSystem.actions.FindAction("Move");
    }


    #region Implement MovementInput
    public override Vector2 ProvideMovementInput()
    {
        if (_ownerPhotonView != null && !_ownerPhotonView.IsMine) return Vector2.zero;
        if (_movementAction == null) return Vector2.zero;
        return _movementAction.ReadValue<Vector2>();
    }
    #endregion
}
