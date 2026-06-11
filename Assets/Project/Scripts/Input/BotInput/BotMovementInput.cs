using UnityEngine;

public class BotMovementInput : MovementInput
{
    private Vector2 _direction;

    // Called from BotBrain to set the simulated horizontal/vertical movement inputs
    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
    }

    #region Implement MovementInput
    public override Vector2 ProvideMovementInput()
    {
        return _direction;
    }
    #endregion
}
