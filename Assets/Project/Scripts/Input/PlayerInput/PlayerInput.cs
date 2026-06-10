using Photon.Pun;
using UnityEngine;

public class PlayerInput : CharacterInput
{
    // 
    #region Implement CharacterInput
    protected override void SetupInput()
    {
        _movementInput = new PlayerMovementInput(GetComponent<PhotonView>());
        _attackInput = new PlayerAttackInput(GetComponent<PhotonView>());
    }
    #endregion
}
