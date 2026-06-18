using UnityEngine;

public abstract class RotationHandler : MonoBehaviour, IComponentDeactiveOnDeath
{
    // 
    protected Transform _target;
    protected float _angle;

    public Transform Target => _target;

    /// <summary>
    /// Calculate angle between owner and target
    /// </summary>
    protected virtual void Update()
    {
        if (_target == null) return;

        Vector2 direction = _target.position - transform.position;
        _angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    public void SetTarget(Transform target) => _target = target;

    #region Explicit implement IComponentDeactiveOnDeath
    void IComponentDeactiveOnDeath.Active() => enabled = true;
    void IComponentDeactiveOnDeath.Deactive() => enabled = false;
    #endregion
}
