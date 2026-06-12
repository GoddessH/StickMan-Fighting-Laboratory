using System;
using UnityEngine;

public abstract class EventInput: IProvider<Func<bool>>
{
    //
    protected Action _cachedSubscriber;

    public abstract void SubscribeInputAction(Action subscriber);
    public abstract void UnsubscribeInputAction();

    #region Implicit implement IProvider
    public abstract Func<bool> Provide();
    #endregion
}
