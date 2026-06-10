using System;
using UnityEngine;

public class AttackState : State
{
    //
    private readonly Vector2 _comboWindow = new Vector2(.45f, /*.7f*/1);
    //private const float _comboWindow = .5f;

    private Func<bool> _onCheckInput;

    private bool _isCorrectAnimation;
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

        _stateData.AnimationHandler.SetBool(AnimationName.Attack, true);
        _isCorrectAnimation = false;
        _stateData.AnimationHandler.SetInteger(AnimationName.AttackCombo, _comboCount = 1);
    }
    public override void UpdateState()
    {
        (bool flag, float time) tick = _stateData.AnimationHandler.CheckCurrentState("Attack1");

        if (!_isCorrectAnimation)
        {
            if (tick.flag) _isCorrectAnimation = true;
        }
        else if (tick.time >= 1) _onComplete?.Invoke(_type);

        if (_isCorrectAnimation && _onCheckInput.Invoke() && _comboCount < 4 && tick.time >= _comboWindow.x && tick.time <= _comboWindow.y)
            _stateData.AnimationHandler.SetInteger(AnimationName.AttackCombo, ++_comboCount);
    }
    public override void ExitState()
    {
        _stateData.AnimationHandler.SetBool(AnimationName.Attack, false);
    }
    #endregion
}
