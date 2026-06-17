public class BotInput : CharacterInput
{
    #region Implement CharacterInput
    protected override void SetupInput()
    {
        _movementInput = new BotMovementInput();
        _attackInput = new BotAttackInput();
        _blockInput = new BotBlockInput();
        _flashInput = new BotFlashInput();
    }
    #endregion

    // Typed accessors so that BotBrain can interface directly without casting
    public BotMovementInput BotMovement => (BotMovementInput)_movementInput;
    public BotAttackInput BotAttack => (BotAttackInput)_attackInput;
    public BotBlockInput BotBlock => (BotBlockInput)_blockInput;
    public BotFlashInput BotFlash => (BotFlashInput)_flashInput;
}
