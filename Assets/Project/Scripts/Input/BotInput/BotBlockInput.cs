using System;

public class BotBlockInput : BlockInput
{
    private bool _isHolding;

    // Called from BotBrain to start blocking
    public void StartBlock()
    {
        _isHolding = true;
        _cachedSubscriber?.Invoke();
    }

    // Called from BotBrain to stop blocking
    public void StopBlock()
    {
        _isHolding = false;
    }

    private bool OnCheckHolding()
    {
        return _isHolding;
    }

    #region Implement BlockInput
    public override void SubscribeBlockAction(Action subscriber)
    {
        _cachedSubscriber = subscriber;
    }

    public override void UnsubscribeBlockAction()
    {
        _cachedSubscriber = null;
    }

    public override Func<bool> Provide()
        => OnCheckHolding;
    #endregion
}
