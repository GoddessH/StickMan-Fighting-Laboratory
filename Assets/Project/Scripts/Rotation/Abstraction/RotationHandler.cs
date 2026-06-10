using UnityEngine;

public abstract class RotationHandler : MonoBehaviour
{
    // 
    protected Transform _target;
    protected float _angle;

    /// <summary>
    /// Calculate angle between owner and target
    /// </summary>
    protected virtual void Update()
    {
        if (_target == null) return;

        Vector2 direction = _target.position - transform.position;
        _angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    public void SetTarget(Transform target)
        => _target = target;
}
