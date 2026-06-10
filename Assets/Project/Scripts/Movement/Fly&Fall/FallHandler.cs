using System;
using UnityEngine;

[Serializable]
public class FallHandler
{
    //
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _distanceThreshold;
    private Transform _ownerTransform;
    private AnimationHandler _animationHandler;

    public void Init(Transform ownerTransform, AnimationHandler animationHandler)
    {
        _ownerTransform = ownerTransform;
        _animationHandler = animationHandler;
    }

    public void Execute()
    {
        bool isFall = !_animationHandler.GetBool(AnimationName.Fly);

        RaycastHit2D rayHit = Physics2D.Raycast(_ownerTransform.position, Vector2.down, _distanceThreshold, _groundMask);
        if (rayHit.collider != null)
        {
            isFall = false;
        }

        _animationHandler.SetBool(AnimationName.Fall, isFall);
    }
}
