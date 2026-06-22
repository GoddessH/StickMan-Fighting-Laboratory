using System;
using UnityEngine;

public class RangeAttackController : AttackController
{
    //
    [SerializeField] private Spawner _projectileSpawner;
    [SerializeField] private AnimationEventReceiver _animationEventReceiver;

    private ProductContext _projectileContext = new ProductContext();

    private void ShootProjectile()
    {
        _projectileContext.Direction = (_projectileSpawner.transform.position - transform.position).normalized;
        _projectileSpawner.Spawn(0, new ProductContext(_projectileContext));
    }

    #region Implement AttackController
    protected void DoDamages(HurtPoint hurtPoint)
    {
        if (hurtPoint == null) return;

        hurtPoint.TakeDamages(_attackDamages);
    }
    #endregion

    #region Override AttackController
    protected override void Awake()
    {
        base.Awake();
        _animationEventReceiver.Init(ShootProjectile);
        _projectileContext.OnDoDamages = DoDamages;
    }
    #endregion
}
