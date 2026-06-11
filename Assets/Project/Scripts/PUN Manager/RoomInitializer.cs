using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInitializer
{
    //
    [SerializeField] private UIInMatchManager _uiInMatchManager;

    /// <param name="fighterStack">The caller should pass the reference of a copy, not the original</param>
    public void InitFighterFacing(Stack<Character> fighterStack)
    {
        if (fighterStack.Count == 2)
        {
            Character fighterA = fighterStack.Pop();
            Character fighterB = fighterStack.Pop();
            fighterA?.GetComponent<RotationHandler>()?.SetTarget(fighterB?.transform);
            fighterB?.GetComponent<RotationHandler>()?.SetTarget(fighterA?.transform);
        }
    }

    public void SetupUI(Stack<Character> fighterStack)
    {
        _uiInMatchManager?.ConnectToFighter(fighterStack);
    }
}
