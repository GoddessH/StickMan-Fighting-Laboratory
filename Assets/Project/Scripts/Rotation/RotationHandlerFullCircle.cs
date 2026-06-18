using UnityEngine;
using UnityEngine.UIElements;

public class RotationHandlerFullCircle : RotationHandler
{
    //
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private IntervalData _balanceThreshold = new IntervalData(1.8f, 2.5f);
    [SerializeField] private float _rayShootRate = .05f;

    private float _balanceWeight = 0;

    private void CalculateBalanceWeight()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.down, _balanceThreshold.Max, _groundMask);

        _balanceWeight = 0;

        if (rayHit.collider == null) return;

        float minPositionY = transform.position.y - _balanceThreshold.Min;
        float distanceY = Mathf.Abs(minPositionY - rayHit.point.y);
        distanceY = _balanceThreshold.GetIntervalLenght() - distanceY;
        _balanceWeight = distanceY / _balanceThreshold.GetIntervalLenght();
    }

    #region Implement RotationHandler
    protected override void Update()
    {
        if (_target == null) return;

        base.Update();

        CalculateBalanceWeight();

        float flipWeight = 1;
        float targetAngle = 0;
        if (_angle < -90 || _angle > 90)
        {
            flipWeight = -1;
            targetAngle = 180;
        }

        transform.localScale = new Vector3(transform.localScale.x, flipWeight * Mathf.Abs(transform.localScale.y), transform.localScale.z);

        float resAngle = Mathf.LerpAngle(_angle, targetAngle, _balanceWeight);
        transform.rotation = Quaternion.Euler(0, 0, resAngle);
    }
    #endregion

#if UNITY_EDITOR
    #region DevLog
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 target = transform.position + Vector3.down * _balanceThreshold.Max;
        Gizmos.DrawRay(transform.position, target - transform.position);
    }
    #endregion
#endif
}
