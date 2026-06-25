using Spine;
using System;

public class HurtState : State
{
    //
    private TrackEntry _currentTrack;
    private Func<float> _onGetLastDamages;
    private Action<float> _onReduceHealth;

    #region Implement State
    protected override void SetContext()
    {
        _onReduceHealth = _ownerGO.GetComponent<HealthManager>()?.Provide();
        _onGetLastDamages = _ownerGO.GetComponent<HurtController>()?.Provide();
    }
    public override void EnterState()
    {
        _currentTrack = _animationHandler.SetAnimation(_animationHandler.Library.HitCombo[0], false);
        if (_currentTrack != null) _currentTrack.Complete += HurtState_Complete;

        if (_onGetLastDamages != null) 
            _onReduceHealth?.Invoke(_onGetLastDamages.Invoke());
    }

    private void HurtState_Complete(TrackEntry trackEntry) => _onComplete?.Invoke(_type);

    public override void UpdateState()
    {
        if (!_animationHandler.IsCurrentAnimationName(_animationHandler.Library.HitCombo[0]))
        {
            _onComplete?.Invoke(_type);
            return;
        }
    }
    public override void ExitState()
    {
        _currentTrack = null;
    }
    #endregion
}
