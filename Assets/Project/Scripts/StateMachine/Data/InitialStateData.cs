using System;
using UnityEngine;

public struct InitialStateData
{
    //
    public GameObject OwnerGO { get; private set; }
    public Action<StateType> OnComplete { get; private set; }
    public StateData StateData { get; private set; }
    public StateType Type { get; private set; }
    public int Priority { get; private set; }

    public InitialStateData(GameObject ownerGO, Action<StateType> onComplete, StateData stateData, StateType type, int priority)
    {
        OwnerGO = ownerGO;
        OnComplete = onComplete;
        StateData = stateData;
        Type = type;
        Priority = priority;
    }
}
