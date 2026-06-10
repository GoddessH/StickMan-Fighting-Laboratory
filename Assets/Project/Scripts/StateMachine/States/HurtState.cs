using System;

public class HurtState : State
{
    //
    private Func<float> _onGetLastDamages;
    private Action<float> _onReduceHealth;

    private bool _isCorrectState;

    #region Implement State
    protected override void SetContext()
    {
        _onReduceHealth = _ownerGO.GetComponent<HealthManager>()?.Provide();
        _onGetLastDamages = _ownerGO.GetComponent<HurtController>()?.Provide();
    }
    public override void EnterState()
    {
        _stateData.AnimationHandler.SetBool(AnimationName.Hurt, true);
        _isCorrectState = false;

        if (_onGetLastDamages != null) 
            _onReduceHealth?.Invoke(_onGetLastDamages.Invoke());
    }
    public override void UpdateState()
    {
        (bool flag, float time) tick = _stateData.AnimationHandler.CheckCurrentState("Hurt");
        if (!_isCorrectState)
        {
            if (tick.flag) _isCorrectState = true;
        }
        else if (tick.time >= 1) _onComplete?.Invoke(_type);
    }
    public override void ExitState()
    {
        _stateData.AnimationHandler.SetBool(AnimationName.Hurt, false);
    }
    #endregion
}
