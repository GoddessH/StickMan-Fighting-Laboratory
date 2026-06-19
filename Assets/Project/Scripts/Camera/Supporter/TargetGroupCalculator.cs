using System;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public class TargetGroupCalculator
{
    //
    [SerializeField] private Camera _camera;
    [SerializeField] private CinemachineGroupFraming _groupFraming;
    [SerializeField] private CinemachineTargetGroup _targetGroup;
    [SerializeField] private float _lerpStep = .2f;

    private TargetGroupData _data;

    private Transform _ownerTransform;
    private Transform _enemyTransform;
    private int _enemyIndex;

    private bool ValidExecution() => 
        _ownerTransform != null && _enemyTransform != null && _camera != null && _targetGroup != null && _groupFraming != null;

    private bool IsEnemyInBound(float absoluteX, float absoluteY)
        => Mathf.Max(absoluteX / _data.BoundZone.width, absoluteY / _data.BoundZone.height) <= 1;

    public void Init(Transform ownerTransform, Transform enemyTransform)
    {
        _ownerTransform = ownerTransform;
        _enemyTransform = enemyTransform;

        if (!ValidExecution()) return;

        _data = new TargetGroupData(_camera);

        _groupFraming.OrthoSizeRange = _data.OrthorSizeRange.ConvertToVector2();

        _targetGroup.AddMember(ownerTransform, _data.OwnerWeight, _data.Radius);
        _targetGroup.AddMember(enemyTransform, _data.EnemyWeight, _data.Radius);
        _enemyIndex = _targetGroup.FindMember(enemyTransform);
    }

    public void Execute()
    {
        if (!ValidExecution()) return;

        Vector2 delta = _enemyTransform.position - _ownerTransform.position;

        float absoluteX = Mathf.Abs(delta.x);
        float absoluteY = Mathf.Abs(delta.y);
        if (!IsEnemyInBound(absoluteX, absoluteY))
        {
            _targetGroup.Targets[_enemyIndex].Weight = 0;
            return;
        }

        float deltaCoordinate = Mathf.Clamp(absoluteX, _data.DeadZone.width, _data.BoundZone.width);
        float deltaY = Mathf.Clamp(absoluteY, _data.DeadZone.height, _data.BoundZone.height);

        float boundLength = _data.BoundZone.width;
        float safeZoneLength = _data.SafeZone.width;

        if (deltaCoordinate < deltaY)
        {
            deltaCoordinate = deltaY;
            boundLength = _data.BoundZone.width;
            safeZoneLength = _data.SafeZone.height;
        }

        float distanceWeight = (boundLength - deltaCoordinate) / safeZoneLength;

        _targetGroup.Targets[_enemyIndex].Weight = _data.EnemyWeight * distanceWeight;
    }
}
