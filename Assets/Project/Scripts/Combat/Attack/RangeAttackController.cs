using System;
using UnityEngine;

public class RangeAttackController : AttackController
{
    //
    [SerializeField] private Transform _shootPivot;
    [SerializeField] private AnimationEventReceiver _animationEventReceiver;

    private ProjectileContext _projectileContext = new ProjectileContext();

    private void ShootProjectile()
    {
        ProjectileSpawner spawner = SpawnerManager.Instance.ProjectileSpawner;
        if (spawner == null || _shootPivot == null) return;

        _projectileContext.SpawnPosition = _shootPivot.position;
        _projectileContext.Direction = (_shootPivot.transform.position - transform.position).normalized;
        spawner.Spawn(new ProjectileContext(_projectileContext));
    }

    private void DoDamages(HurtPoint hurtPoint)
    {
        if (hurtPoint == null) return;

        hurtPoint.TakeDamages(_attackDamages);
    }

    #region Override AttackController
    protected override void Awake()
    {
        base.Awake();
        _animationEventReceiver.Init(ShootProjectile);
        _projectileContext.OnDoDamages = DoDamages;
    }
    #endregion
}
