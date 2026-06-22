using UnityEngine;

public class TargetGroupData
{
    //
    #region Const data
    public Interval OrthorSizeRange { get; private set; }
    public float OwnerWeight { get; private set; }
    public float EnemyWeight { get; private set; }
    public float Radius { get; private set; }
    #endregion

    #region Calculated Data
    public (float width, float height) BoundZone { get; private set; }
    public (float width, float height) DeadZone { get; private set; }
    public (float width, float height) SafeZone { get; private set; }
    #endregion

    public TargetGroupData(Camera camera)
    {
        InitConstData();

        if (camera == null) return;
        InitCalculatedData(camera.aspect);
    }

    private void InitConstData()
    {
        OwnerWeight = .8f;
        EnemyWeight = .6f;
        Radius = 4f;
        OrthorSizeRange = new Interval(5, 7);
    }

    private void InitCalculatedData(float aspect)
    {
        BoundZone = new (OrthorSizeRange.Max * aspect, OrthorSizeRange.Max);
        DeadZone = new (OrthorSizeRange.Min * aspect, OrthorSizeRange.Min);
        SafeZone = new (BoundZone.width - DeadZone.width, BoundZone.height - DeadZone.height);
    }
}
