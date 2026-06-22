using UnityEngine;

public class MeleeAttackController : AttackController
{
    //
    [SerializeField] private AttackHitBox _attackHitBox;
    [SerializeField] private AnimationEventReceiver _attackEventReceiver;

    protected override void Awake()
    {
        base.Awake();

        _attackEventReceiver?.Init(DoDamages);
    }

    #region Callback in animation's event
    protected void DoDamages()
    {
        HurtPoint hurtPoint = _attackHitBox?.DetectTarget();

        if (hurtPoint == null) return;
        hurtPoint.TakeDamages(_attackDamages);
        _onHit?.Invoke();
    }
    #endregion
}
