using System;
using UnityEngine;

public class DeadState : State
{
    //
    private Action<bool> _onSetEnableHurtPoint;
    private bool _isCorrectState;

    #region Implement State
    protected override void SetContext()
    {
        _onSetEnableHurtPoint = _ownerGO.GetComponent<DeadController>()?.Provide();
    }
    public override void EnterState()
    {
        _isCorrectState = false;

        _stateData.AnimationHandler.SetBool(AnimationName.Die, true);
        _onSetEnableHurtPoint?.Invoke(false);
    }

    public override void UpdateState()
    {
        (bool flag, float time) tick = _stateData.AnimationHandler.CheckCurrentState("Die");

        if (!_isCorrectState)
        {
            if (tick.flag) _isCorrectState = true;
        }
    }

    public override void ExitState()
    {
        _stateData.AnimationHandler.SetBool(AnimationName.Die, false);
        _onSetEnableHurtPoint?.Invoke(true);
    }
    #endregion
}
