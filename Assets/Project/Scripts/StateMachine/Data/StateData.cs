using UnityEngine;

public struct StateData
{
    //
    public AnimationHandle AnimationHandler { get; private set; }

    public StateData(AnimationHandle animationHandler)
    {
        AnimationHandler = animationHandler;
    }
}
