using System;

public class BotSkillInput : EventInput
{
    private bool _isPressed;

    public void TriggerSkill()
    {
        _isPressed = true;
        _cachedSubscriber?.Invoke();
    }

    public void Clear()
    {
        _isPressed = false;
    }

    private bool OnCheckInput()
    {
        return _isPressed;
    }

    #region Implement EventInput
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
