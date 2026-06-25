using UnityEngine;

public class AttackControllerMelee : AttackController
{
    //
    [SerializeField] private AttackHitBox _attackHitBox;

    #region Implement AttackController
    protected override void Attack()
    {
        HurtPoint hurtPoint = _attackHitBox?.DetectTarget();

        if (hurtPoint == null) return;
        hurtPoint.TakeDamages(_attackDamages);
        _onHit?.Invoke();
    }
    #endregion
}
