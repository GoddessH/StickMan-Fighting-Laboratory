public class BotInput : CharacterInput
{
    #region Implement CharacterInput
    protected override void SetupInput()
    {
        _movementInput = new BotMovementInput();
        _attackInput = new BotAttackInput();
        _blockInput = new BotBlockInput();
        _flashInput = new BotFlashInput();
        
        _skillInputs.Clear();
        _skillInputs.Add(new BotSkillInput());
        _skillInputs.Add(new BotSkillInput());
        _skillInputs.Add(new BotSkillInput());
        _skillInputs.Add(new BotSkillInput());
    }
    #endregion

    // Typed accessors so that BotBrain can interface directly without casting
    public BotMovementInput BotMovement => (BotMovementInput)_movementInput;
    public BotAttackInput BotAttack => (BotAttackInput)_attackInput;
    public BotBlockInput BotBlock => (BotBlockInput)_blockInput;
    public BotFlashInput BotFlash => (BotFlashInput)_flashInput;
    public BotSkillInput BotSkill1 => _skillInputs.Count > 0 ? (BotSkillInput)_skillInputs[0] : null;
    public BotSkillInput BotSkill2 => _skillInputs.Count > 1 ? (BotSkillInput)_skillInputs[1] : null;
    public BotSkillInput BotSkill3 => _skillInputs.Count > 2 ? (BotSkillInput)_skillInputs[2] : null;
    public BotSkillInput BotSkill4 => _skillInputs.Count > 3 ? (BotSkillInput)_skillInputs[3] : null;
}
