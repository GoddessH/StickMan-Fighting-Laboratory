using System;
using UnityEngine;

public class IdleState : State
{
    //

    #region Implement State
    protected override void SetContext()
    {
        
    }

    public override void EnterState()
    {
        _stateData.AnimationHandler.SetBool(AnimationName.Idle, true);
    }
    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        _stateData.AnimationHandler.SetBool(AnimationName.Idle, false);
    }
    #endregion
}
