using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // 
    #region Supporter
    [SerializeField] private TargetGroupCalculator _targetGroupCalculator = new TargetGroupCalculator();
    #endregion

    private void Update()
    {
        _targetGroupCalculator.Execute();
    }

    private void SetFollowTarget(Transform owner, Transform enemy)
    {
        _targetGroupCalculator.Init(owner, enemy);
    }

    public void Setup(Transform owner, Transform enemy)
    {
        if (owner == null) return;
        SetFollowTarget(owner, enemy);
    }
}
