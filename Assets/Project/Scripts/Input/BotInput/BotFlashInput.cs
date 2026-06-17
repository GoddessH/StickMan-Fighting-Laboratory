using System;

public class BotFlashInput : EventInput
{
    private bool _isHolding;

    public void TriggerFlash()
    {
        _isHolding = true;
        _cachedSubscriber?.Invoke();
        _isHolding = false;
    }

    private bool OnCheckInput()
    {
        return _isHolding;
    }

    #region Implement FlashInput
    public override void SubscribeInputAction(Action subscriber)
    {
        _cachedSubscriber = subscriber;
    }

    public override void UnsubscribeInputAction()
    {
        _cachedSubscriber = null;
    }

    public override Func<bool> Provide()
        => OnCheckInput;
    #endregion
}
