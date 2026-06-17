using UnityEngine;

public class RotationHandlerFlip : RotationHandler
{
    //
    #region Implement RotationHandler
    protected override void Update()
    {
        base.Update();

        float flipWeight = 1;

        if (_angle < -90 || _angle > 90) flipWeight = -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * flipWeight, transform.localScale.y, transform.localScale.z);
        ////A
    }
    #endregion
}
