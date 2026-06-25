using System;
using UnityEngine;

[Serializable]
public struct Interval
{
    //
    [SerializeField] private float _min;
    [SerializeField] private float _max;

    public float Min => _min;
    public float Max => _max;

    public Interval(float min, float max)
    {
        _min = min;
        _max = max;
    }

    public Vector2 ConvertToVector2() => new Vector2(_min, _max);
    public bool IsInRange(float value) => value >= _min && value <= _max;
    public float GetLength() => Mathf.Abs(_max - _min);
}
