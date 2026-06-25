using UnityEngine;

public class FlyController : MonoBehaviour, IFlyChecker
{
    //
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _flyThreshold = 1.8f;

    private bool _isFly = false;

    private void Update()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.down, _flyThreshold, _groundMask);
        if (rayHit.collider != null) _isFly = false;
        else _isFly = true;
    }

    #region Explicit implement IFlyChecker
    bool IFlyChecker.IsFly() => _isFly;
    #endregion


#if UNITY_EDITOR
    #region DevLog
    [SerializeField] private bool _showLog;

    private void OnDrawGizmos()
    {
        if (!_showLog) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector2.down * _flyThreshold);
    }
    #endregion
#endif
}
