using System;

public abstract class FlashInput : IProvider<Func<bool>>
{
    protected Action _cachedSubscriber;

    public abstract void SubscribeFlashAction(Action subscriber);
    public abstract void UnsubscribeFlashAction();

    #region Implement IProvider
    public abstract Func<bool> Provide();
    #endregion
}
