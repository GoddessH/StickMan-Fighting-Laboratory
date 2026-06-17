using Photon.Pun;
using UnityEngine;

public abstract class CharacterInput: MonoBehaviourPun
{
    //
    protected MovementInput _movementInput;
    protected EventInput _attackInput;
    protected EventInput _blockInput;
    protected EventInput _chargeInput;
    protected EventInput _flashInput;
    protected EventInput _skill1Input;
    protected EventInput _skill2Input;
    protected EventInput _skill3Input;
    protected EventInput _skill4Input;

    private bool _hasSetup = false;

    public MovementInput MovementInput 
    {
        get
        {
            EnsureSetup();
            return _movementInput;
        }
    }

    public EventInput AttackInput 
    {
        get
        {
            EnsureSetup();
            return _attackInput;
        }
    }

    public EventInput BlockInput 
    {
        get
        {
            EnsureSetup();
            return _blockInput;
        }
    }

    public EventInput FlashInput 
    {
        get
        {
            EnsureSetup();
            return _flashInput;
        }
    }

    public EventInput ChargeInput => _chargeInput;

    public EventInput Skill1Input 
    {
        get
        {
            EnsureSetup();
            return _skill1Input;
        }
    }

    public EventInput Skill2Input 
    {
        get
        {
            EnsureSetup();
            return _skill2Input;
        }
    }

    public EventInput Skill3Input 
    {
        get
        {
            EnsureSetup();
            return _skill3Input;
        }
    }

    public EventInput Skill4Input 
    {
        get
        {
            EnsureSetup();
            return _skill4Input;
        }
    }

    protected virtual void Awake()
        => EnsureSetup();

    private void EnsureSetup()
    {
        if (!_hasSetup)
        {
            SetupInput();
            _hasSetup = true;
        }
    }

    protected abstract void SetupInput();
}
