using System;
using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    // 
    //[SerializeField] HurtPoint _ownedHurtPoint;
    [SerializeField] private LayerMask _damageableMask;
    [SerializeField] private float _attackRadius;

    public HurtPoint DetectTarget()
    {
        //Collider2D[] targetCollider = Physics2D.OverlapCircleAll(transform.position, _attackRadius, _damageableMask);

        //Collider2D target = null;
        //foreach(var collider in targetCollider)
        //{
        //    if (collider.gameObject == _ownedHurtPoint.gameObject) continue;
        //    target = collider;
        //}

        //return target?.GetComponent<HurtPoint>();

        Collider2D target = Physics2D.OverlapCircle(transform.position, _attackRadius, _damageableMask);
        return target?.GetComponent<HurtPoint>();
    }

#if UNITY_EDITOR
    #region DevLog
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }
    #endregion
#endif
}
