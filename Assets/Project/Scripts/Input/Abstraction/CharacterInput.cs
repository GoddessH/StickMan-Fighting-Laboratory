using Photon.Pun;
using UnityEngine;

public abstract class CharacterInput: MonoBehaviourPun
{
    //
    protected MovementInput _movementInput;
    protected AttackInput _attackInput;
    protected BlockInput _blockInput;
    protected FlashInput _flashInput;

    private bool _hasSetup = false;

    public MovementInput MovementInput 
    {
        get
        {
            EnsureSetup();
            return _movementInput;
        }
    }

    public AttackInput AttackInput 
    {
        get
        {
            EnsureSetup();
            return _attackInput;
        }
    }

    public BlockInput BlockInput 
    {
        get
        {
            EnsureSetup();
            return _blockInput;
        }
    }

    public FlashInput FlashInput 
    {
        get
        {
            EnsureSetup();
            return _flashInput;
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
