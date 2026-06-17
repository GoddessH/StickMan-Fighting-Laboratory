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
    //protected FlashInput _flashInput;

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
