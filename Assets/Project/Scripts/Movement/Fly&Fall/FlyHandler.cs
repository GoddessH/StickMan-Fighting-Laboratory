using System;
using UnityEngine;

[Serializable]
public class FlyHandler
{
    //
    private AnimationHandler _animationHandler;


    public void Init(AnimationHandler animationHandler)
    {
        _animationHandler = animationHandler;
    }

    public void Execute(float yInput)
    {

        bool isFly = false;
        if (yInput > 0) isFly = true;

        _animationHandler.SetBool(AnimationName.Fly, isFly);
    }
}
