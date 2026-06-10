using System;
using UnityEngine;

public class HurtPoint : MonoBehaviour
{
    //
    private Action<float> _onHurt;

    public void Init(Action<float> onHurt)
    {
        _onHurt = onHurt;
    }

    public void TakeDamages(float amount)
        => _onHurt?.Invoke(amount);
}
