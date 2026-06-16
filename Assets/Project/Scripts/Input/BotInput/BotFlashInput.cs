using System;

public class BotFlashInput : FlashInput
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
    public override void SubscribeFlashAction(Action subscriber)
    {
        _cachedSubscriber = subscriber;
    }

    public override void UnsubscribeFlashAction()
    {
        _cachedSubscriber = null;
    }

    public override Func<bool> Provide()
        => OnCheckInput;
    #endregion
}
