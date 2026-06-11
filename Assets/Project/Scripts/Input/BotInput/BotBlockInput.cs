using System;

public class BotBlockInput : EventInput
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
    public override void SubscribeInputAction(Action subscriber)
    {
        _cachedSubscriber = subscriber;
    }

    public override void UnsubscribeInputAction()
    {
        _cachedSubscriber = null;
    }

    public override Func<bool> Provide()
        => OnCheckHolding;
    #endregion
}
