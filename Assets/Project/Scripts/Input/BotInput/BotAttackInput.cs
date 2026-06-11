using System;

public class BotAttackInput : AttackInput
{
    private bool _isPressed;

    // Called from BotBrain to trigger the attack event
    public void TriggerAttack()
    {
        _isPressed = true;
        _cachedSubscriber?.Invoke();
    }

    // Called from BotBrain to clear the input state for the next frame
    public void Clear()
    {
        _isPressed = false;
    }

    private bool OnCheckInput()
    {
        return _isPressed;
    }

    #region Implement AttackInput
    public override void SusbscribeAttackAction(Action subscriber)
    {
        _cachedSubscriber = subscriber;
    }

    public override void UnsubscribeAttackAction()
    {
        _cachedSubscriber = null;
    }

    public override Func<bool> Provide()
        => OnCheckInput;
    #endregion
}
