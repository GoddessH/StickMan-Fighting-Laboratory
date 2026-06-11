using Photon.Pun;
using UnityEngine;

public abstract class CharacterInput: MonoBehaviourPun
{
    //
    protected MovementInput _movementInput;
    protected AttackInput _attackInput;
    protected BlockInput _blockInput;

    public MovementInput MovementInput => _movementInput;
    public AttackInput AttackInput => _attackInput;
    public BlockInput BlockInput => _blockInput;

    protected virtual void Awake()
        => SetupInput();

    protected abstract void SetupInput();
}
