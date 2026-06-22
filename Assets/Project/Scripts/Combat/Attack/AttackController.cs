using System;
using UnityEngine;

[RequireComponent(typeof(CharacterInput))]
public abstract class AttackController : MonoBehaviour, IProvider<Func<bool>>, IDamageDealerEvent
{
    //
    [SerializeField] protected float _attackDamages;

    protected IStateRequestReceiver _requestReceiver;
    protected EventInput _attackInput;

    protected Action _onHit;

    #region Supporter
    protected StateRequester _attackRequester;
    #endregion

    protected virtual void Awake()
    {
        _requestReceiver = GetComponent<IStateRequestReceiver>();
        _attackRequester = new StateRequester(StateType.Attack);

        _attackInput = GetComponent<CharacterInput>().AttackInput;
    }

    protected virtual void OnEnable()
        => _attackInput.SubscribeInputAction(Attack);

    protected virtual void OnDisable()
        => _attackInput.UnsubscribeInputAction();

    protected virtual void Attack()
        => _attackRequester?.RequestState(_requestReceiver);

    #region Implement IProvider
    /// <summary>
    /// Provide to AttackState
    /// </summary>
    public Func<bool> Provide()
        => _attackInput.Provide();
    #endregion

    #region Explicit implement IDamageDealerEvent
    void IDamageDealerEvent.SubscribeEvent(Action subscriber)
    {
        _onHit += subscriber;
    }
    void IDamageDealerEvent.UnsubscribeEvent(Action subscriber)
    {
        _onHit -= subscriber;
    }
    #endregion

}
