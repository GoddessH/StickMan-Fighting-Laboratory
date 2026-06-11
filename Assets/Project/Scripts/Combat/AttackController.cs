using System;
using UnityEngine;

[RequireComponent(typeof(CharacterInput))]
public class AttackController : MonoBehaviour, IProvider<Func<bool>>, IDamageDealerEvent
{
    //
    [SerializeField] private AttackHitBox _attackHitBox;
    [SerializeField] private float _attackDamages;

    private IStateRequestReceiver _requestReceiver;
    private EventInput _attackInput;

    private Action _onHit;

    #region Supporter
    private StateRequester _attackRequester;
    #endregion

    private void Awake()
    {
        _requestReceiver = GetComponent<IStateRequestReceiver>();
        _attackRequester = new StateRequester(StateType.Attack);

        _attackInput = GetComponent<CharacterInput>().AttackInput;

        AnimationEventReceiver attackEventReceiver = ComponentEnsurer.EnsureComponent(GetComponent<AnimationEventReceiver>(), gameObject);

        attackEventReceiver.Init(DoDamages);
    }

    private void OnEnable()
        => _attackInput.SubscribeInputAction(Attack);

    private void OnDisable() 
        => _attackInput.UnsubscribeInputAction();

    private void Attack() 
        => _attackRequester?.RequestState(_requestReceiver);

    #region Call in animation's event
    private void DoDamages()
    {
        HurtPoint hurtPoint = _attackHitBox?.DetectTarget();

        if (hurtPoint == null) return;
        hurtPoint.TakeDamages(_attackDamages);
        _onHit?.Invoke();
    }
    #endregion

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
