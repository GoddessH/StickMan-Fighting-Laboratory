using System;
using UnityEngine;

public class AttackControllerRange : AttackController
{
    //
    [SerializeField] private Transform _shootPivot;

    private ProjectileContext _projectileContext = new ProjectileContext();

    private void DoDamages(HurtPoint hurtPoint)
    {
        if (hurtPoint == null) return;

        hurtPoint.TakeDamages(_attackDamages);
    }

    #region Implement AttackController
    protected override void Attack()
    {
        ProjectileSpawner spawner = SpawnerManager.Instance.ProjectileSpawner;
        if (spawner == null || _shootPivot == null) return;

        _projectileContext.SpawnPosition = _shootPivot.position;
        _projectileContext.Direction = (_shootPivot.transform.position - transform.position).normalized;
        spawner.Spawn(new ProjectileContext(_projectileContext));
    }
    #endregion

    #region Override AttackController
    protected override void Awake()
    {
        base.Awake();
        _projectileContext.OnDoDamages = DoDamages;
    }
    #endregion
}
