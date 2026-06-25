using Spine.Unity;
using UnityEngine;

public class IdleState : State
{
    //
    private IFlyChecker _flyChecker;

    #region Implement State
    protected override void SetContext()
    {
        _flyChecker = _ownerGO.GetComponent<IFlyChecker>();
    }

    public override void EnterState()
    {
        if (_flyChecker == null) return;

        AnimationReferenceAsset animation = _animationHandler.Library.IdleToggle.PrimaryAnimation;
        if (_flyChecker.IsFly()) animation = _animationHandler.Library.IdleToggle.SecondaryAnimation;

        _animationHandler.SetAnimation(animation, true);
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
    }
    #endregion
}
