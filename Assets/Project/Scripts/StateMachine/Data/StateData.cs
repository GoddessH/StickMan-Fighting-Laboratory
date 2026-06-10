using UnityEngine;

public struct StateData
{
    //
    public AnimationHandler AnimationHandler { get; private set; }

    public StateData(AnimationHandler animationHandler)
    {
        AnimationHandler = animationHandler;
    }
}
