using System;
using UnityEngine;

[Serializable]
public struct IntervalData
{
    //
    [SerializeField] private float _min;
    [SerializeField] private float _max;

    public float Min => _min;
    public float Max => _max;

    public IntervalData(float min, float max)
    {
        _min = min;
        _max = max;
    }

    public float GetIntervalLenght() => Mathf.Abs(_max - _min);
}
