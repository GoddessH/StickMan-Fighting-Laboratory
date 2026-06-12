using System;
using UnityEngine;

public abstract class EventInput: IProvider<Func<bool>>
{
    //
    protected Action _cachedSubscriber;

    /// <summary>
    /// Subscribe an Action called back whenever Input is fired
    /// </summary>
    public abstract void SubscribeInputAction(Action subscriber);

    /// <summary>
    /// Unsubcribe current Action
    /// </summary>
    public abstract void UnsubscribeInputAction();

    #region Implicit implement IProvider
    public abstract Func<bool> Provide();
    #endregion
}
