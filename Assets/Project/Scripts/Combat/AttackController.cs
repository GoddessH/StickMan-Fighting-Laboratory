using System;
using UnityEngine;

[RequireComponent(typeof(CharacterInput))]
public class AttackController : MonoBehaviour, IProvider<Func<bool>>
{
    //
    [SerializeField] private AttackHitBox _attackHitBox;
    [SerializeField] private float _attackDamages;

    private IStateRequestReceiver _requestReceiver;
    private AttackInput _attackInput;

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
        => _attackInput.SusbscribeAttackAction(Attack);

    private void OnDisable() 
        => _attackInput.UnsubscribeAttackAction();

    private void Attack() 
        => _attackRequester?.RequestState(_requestReceiver);

    #region Call in animation's event
    private void DoDamages()
        => _attackHitBox?.DetectTarget()?.TakeDamages(_attackDamages);
    #endregion

    #region Implement IProvider
    /// <summary>
    /// Provide to AttackState
    /// </summary>
    public Func<bool> Provide()
        => _attackInput.Provide();
    #endregion

}
