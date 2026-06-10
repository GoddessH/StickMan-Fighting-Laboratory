using Photon.Pun;
using UnityEngine;

public abstract class CharacterInput: MonoBehaviourPun
{
    //
    protected MovementInput _movementInput;
    protected AttackInput _attackInput;

    public MovementInput MovementInput => _movementInput;
    public AttackInput AttackInput => _attackInput;

    protected virtual void Awake()
        => SetupInput();

    protected abstract void SetupInput();
}
