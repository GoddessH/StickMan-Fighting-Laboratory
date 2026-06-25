using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CharacterInput))]
public class MovementController : MonoBehaviour, IMover, IProvider<Func<Vector2>>
{
    //
    [SerializeField] private float _movementSpeed;
    private MovementInput _movementInput;
    private Rigidbody2D _rigidBody;
    private IStateRequestReceiver _requestReceiver;

    private Vector2 _rawInput;

    #region Supporter
    private StateRequester _moveRequester;
    #endregion

    private void Awake()
    {
        _requestReceiver = GetComponent<IStateRequestReceiver>();
        _moveRequester = new StateRequester(StateType.Move);

        _rigidBody = GetComponent<Rigidbody2D>();
        _movementInput = GetComponent<CharacterInput>().MovementInput;
    }

    private void Update()
    {
        if (_movementInput == null) return;

        _rawInput = _movementInput.ProvideMovementInput();
        if (_rawInput != Vector2.zero) _moveRequester?.RequestState(_requestReceiver);
    }

    #region Explicit implement IMover
    void IMover.Move() => _rigidBody.linearVelocity = _rawInput.normalized * _movementSpeed;
    void IMover.StopMove() => _rigidBody.linearVelocity = Vector2.zero;
    #endregion

    #region Implicit implement IProvider
    public Func<Vector2> Provide() => () => _rawInput;
    #endregion
}
