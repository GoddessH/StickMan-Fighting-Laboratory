using Photon.Pun;
using UnityEngine;

public class PlayerInput : CharacterInput
{
    // 
    #region Implement CharacterInput
    protected override void SetupInput()
    {
        PhotonView pv = GetComponent<PhotonView>();
        _movementInput = new PlayerMovementInput(pv);
        _attackInput = new PlayerAttackInput(pv);
        _blockInput = new PlayerBlockInput(pv);
        _flashInput = new PlayerFlashInput(pv);
        _chargeInput = new PlayerChargeInput(pv);
        _skill1Input = new PlayerSkillInput(pv, InputActionName.Skill1Action);
        _skill2Input = new PlayerSkillInput(pv, InputActionName.Skill2Action);
        _skill3Input = new PlayerSkillInput(pv, InputActionName.Skill3Action);
        _skill4Input = new PlayerSkillInput(pv, InputActionName.Skill4Action);
    }
    #endregion
}
