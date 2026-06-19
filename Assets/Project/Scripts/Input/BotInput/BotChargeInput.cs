using System;

public class BotChargeInput : EventInput
{
    private bool _isHolding;

    // Gọi từ BotBrain để bắt đầu sạc mana
    public void StartCharge()
    {
        _isHolding = true;
        _cachedSubscriber?.Invoke();
    }

    // Gọi từ BotBrain để dừng sạc mana
    public void StopCharge()
    {
        _isHolding = false;
    }

    private bool OnCheckHolding()
    {
        return _isHolding;
    }

    #region Implement ChargeInput
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
