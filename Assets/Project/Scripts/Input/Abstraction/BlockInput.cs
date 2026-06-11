using System;

public abstract class BlockInput : IProvider<Func<bool>>
{
    //
    protected Action _cachedSubscriber;

    public abstract void SubscribeBlockAction(Action subscriber);
    public abstract void UnsubscribeBlockAction();

    #region Implement IProvider
    public abstract Func<bool> Provide();
    #endregion
}
