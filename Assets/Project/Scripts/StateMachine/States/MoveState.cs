using System;
using UnityEngine;

public class MoveState : State
{
    //
    private Func<bool> _onMove;
    private Action _onResetState;

    #region Implement State
    protected override void SetContext()
    {
        MovementController movementController = _ownerGO.GetComponent<MovementController>();
        if (movementController == null) return;

        (Func<bool>, Action) context = movementController.Provide();
        _onMove = context.Item1;
        _onResetState = context.Item2;
    }

    public override void EnterState()
    {
        if (_onMove == null || _onResetState == null)
        {
            _onComplete?.Invoke(_type);
            return;
        }

        _stateData.AnimationHandler.SetBool(AnimationName.Run, true);
    }

    public override void UpdateState()
    {
        if (!_onMove.Invoke()) _onComplete?.Invoke(_type);
    }

    public override void ExitState()
    {
        _stateData.AnimationHandler.SetBool(AnimationName.Run, false);
        _onResetState?.Invoke();
    }
    #endregion
}
