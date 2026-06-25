using Spine;
using System;
using UnityEngine;

public class AttackState : State
{
    //
    private readonly Interval _comboWindow = new Interval(.45f, /*.7f*/1);

    private TrackEntry _currentTrack;
    private Func<bool> _onCheckInput;

    private int _comboCount;

    #region Implement State
    protected override void SetContext()
    {
        _onCheckInput = _ownerGO.GetComponent<AttackController>()?.Provide();
    }
    public override void EnterState()
    {
        if (_onCheckInput == null)
        {
            _onComplete?.Invoke(_type);
            return;
        }

        _comboCount = 0;
        _animationHandler.SetAnimation(_animationHandler.Library.AttackCombo[_comboCount], false);
        _currentTrack = _animationHandler.GetCurrentTrack();
    }
    public override void UpdateState()
    {
        if (_currentTrack == null || _currentTrack.IsComplete)
        {
            _onComplete?.Invoke(_type);
            return;
        }

        float currentPercent = _currentTrack.AnimationTime / _currentTrack.Animation.Duration;
        if (_comboWindow.IsInRange(currentPercent) && _onCheckInput != null && _onCheckInput.Invoke())
        {
            _animationHandler.SetAnimation(_animationHandler.Library.AttackCombo[++_comboCount], false);
            _currentTrack = _animationHandler.GetCurrentTrack();
        }

    }
    public override void ExitState()
    {
    }
    #endregion
}
