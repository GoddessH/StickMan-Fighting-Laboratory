using Spine;
using Spine.Unity;
using System;
using UnityEngine;

public class MoveState : State
{
    //
    private CharacterVisualRoot _visualRoot;
    private TrackEntry _currentTrack;
    private IMover _mover;
    private Func<Vector2> _onGetInput;

    private void SetAnimation()
    {
        if (_visualRoot == null || _onGetInput == null) return;

        AnimationReferenceAsset animation = _animationHandler.Library.MoveToggle.PrimaryAnimation;


        float weight = Vector2.Dot(_onGetInput.Invoke(), _visualRoot.FacingDirection.normalized);

        if (weight < 0) animation = _animationHandler.Library.MoveToggle.SecondaryAnimation;

        _animationHandler.SetAnimation(animation, true);

        //Debug.Log($"Direction: {_visualRoot.FacingDirection.normalized} ----- Weight: {weight}");
    }

    #region Implement State
    protected override void SetContext()
    {
        _mover = _ownerGO.GetComponent<IMover>();
        _visualRoot = _ownerGO.GetComponent<Character>()?.VisualRoot;
        MovementController movementController = _ownerGO.GetComponent<MovementController>();
        if (movementController == null) return;

        _onGetInput = movementController.Provide();
    }

    public override void EnterState()
    {
        if (_mover == null || _onGetInput == null)
        {
            _onComplete?.Invoke(_type);
            return;
        }

        SetAnimation();
    }

    public override void UpdateState()
    {
        if (_mover == null || _onGetInput == null || _onGetInput.Invoke() == Vector2.zero)
        {
            _onComplete?.Invoke(_type);
            return;
        }

        _mover.Move();
        SetAnimation();
    }

    public override void ExitState()
    {
        _mover?.StopMove();
    }
    #endregion
}
