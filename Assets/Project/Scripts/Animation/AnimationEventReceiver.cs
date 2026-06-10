using System;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    //
    private Action _onReceiveAnimationEvent;
    public void Init(Action onReceiveAnimationEvent)
        => _onReceiveAnimationEvent = onReceiveAnimationEvent;

    #region Callback in AnimationEvent
    public void ReceiveAnimationEvent()
        => _onReceiveAnimationEvent?.Invoke();
    #endregion
}
