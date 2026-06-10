using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CharacterInput))]
public class MovementController : MonoBehaviour, IProvider<(Func<bool>, Action)>
{
    //
    [SerializeField] private float _movementSpeed;
    private MovementInput _movementInput;
    private Rigidbody2D _rigidBody;
    private IStateRequestReceiver _requestReceiver;

    private Vector2 _rawInput;

    #region Supporter
    [SerializeField] private FallHandler _fallHandler = new FallHandler();
    private FlyHandler _flyHandler = new FlyHandler();
    private StateRequester _moveRequester;
    #endregion

    private void Awake()
    {
        _requestReceiver = GetComponent<IStateRequestReceiver>();
        _moveRequester = new StateRequester(StateType.Move);

        _rigidBody = GetComponent<Rigidbody2D>();
        _movementInput = GetComponent<CharacterInput>().MovementInput;

        AnimationHandler animationHandler = ComponentEnsurer.EnsureComponent(GetComponent<AnimationHandler>(), gameObject);
        _flyHandler.Init(animationHandler);
        _fallHandler.Init(transform, animationHandler);

    }

    private void Update()
    {
        if (_movementInput == null) return;

        _rawInput = _movementInput.ProvideMovementInput();
        if (_rawInput != Vector2.zero) _moveRequester?.RequestState(_requestReceiver);

        _fallHandler.Execute();
        _flyHandler.Execute(_rawInput.y);
    }

    private bool Move()
    {
        if (_rawInput == Vector2.zero)
        {
            _rigidBody.linearVelocity = Vector2.zero;
            return false;
        }

        _rigidBody.linearVelocity = _rawInput.normalized * _movementSpeed;

        return true;
    }

    private void ResetMovementSpeed()
        => _rigidBody.linearVelocity = Vector2.zero;


    #region Implement IProvider
    /// <summary>
    /// Provide MoveState
    /// </summary>
    /// <returns></returns>
    public (Func<bool>, Action) Provide()
        => (Move, ResetMovementSpeed);
    #endregion
}
