using UnityEngine;

public class RotationHandlerFullCircle : RotationHandler
{
    //
    #region Implement RotationHandler
    protected override void Update()
    {
        if (_target == null) return;

        base.Update();

        float flipWeight = 1;

        if (_angle < -90 || _angle > 90) flipWeight = -1;
        transform.localScale = new Vector3(transform.localScale.x, flipWeight * Mathf.Abs(transform.localScale.y), transform.localScale.z);

        transform.rotation = Quaternion.Euler(0, 0, _angle);
    }
    #endregion
}
