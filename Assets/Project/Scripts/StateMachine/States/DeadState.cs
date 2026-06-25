using System;
using UnityEngine;

public class DeadState : State
{
    //
    private Action<bool> _onSetEnableHurtPoint;

    #region Implement State
    protected override void SetContext()
    {
        _onSetEnableHurtPoint = _ownerGO.GetComponent<DeadController>()?.Provide();
    }
    public override void EnterState()
    {
        _animationHandler.SetAnimation(_animationHandler.Library.Dead, false);
        _onSetEnableHurtPoint?.Invoke(false);
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        _onSetEnableHurtPoint?.Invoke(true);
    }
    #endregion
}
