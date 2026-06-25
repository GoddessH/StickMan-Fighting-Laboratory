using Spine;
using System;
using UnityEngine;

public abstract class State
{
    //
    protected GameObject _ownerGO;
    protected AnimationHandle _animationHandler;
    protected StateType _type;
    protected Action<StateType> _onComplete;

    public int Priority { get; private set; }

    public void Init(InitialStateData initialData)
    {
        _ownerGO = initialData.OwnerGO;
        _type = initialData.Type;
        _animationHandler = initialData.StateData.AnimationHandler;
        _onComplete = initialData.OnComplete;

        Priority = initialData.Priority;
        SetContext();
    }

    protected abstract void SetContext();
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
