using System;
using UnityEngine;

public abstract class AttackInput: IProvider<Func<bool>>
{
    //
    protected Action _cachedSubscriber;

    public abstract void SusbscribeAttackAction(Action subscriber);
    public abstract void UnsubscribeAttackAction();

    #region Implement IProvider
    public abstract Func<bool> Provide();
    #endregion
}
