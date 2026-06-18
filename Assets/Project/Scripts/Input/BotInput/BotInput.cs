public class BotInput : CharacterInput
{
    #region Implement CharacterInput
    protected override void SetupInput()
    {
        _movementInput = new BotMovementInput();
        _attackInput = new BotAttackInput();
        _blockInput = new BotBlockInput();
        _flashInput = new BotFlashInput();
        _skill1Input = new BotSkillInput();
        _skill2Input = new BotSkillInput();
        _skill3Input = new BotSkillInput();
        _skill4Input = new BotSkillInput();
    }
    #endregion

    // Typed accessors so that BotBrain can interface directly without casting
    public BotMovementInput BotMovement => (BotMovementInput)_movementInput;
    public BotAttackInput BotAttack => (BotAttackInput)_attackInput;
    public BotBlockInput BotBlock => (BotBlockInput)_blockInput;
    public BotFlashInput BotFlash => (BotFlashInput)_flashInput;
    public BotSkillInput BotSkill1 => (BotSkillInput)_skill1Input;
    public BotSkillInput BotSkill2 => (BotSkillInput)_skill2Input;
    public BotSkillInput BotSkill3 => (BotSkillInput)_skill3Input;
    public BotSkillInput BotSkill4 => (BotSkillInput)_skill4Input;
}
