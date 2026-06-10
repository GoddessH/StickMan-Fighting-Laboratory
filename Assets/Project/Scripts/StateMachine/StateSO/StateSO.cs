using UnityEngine;

public abstract class StateSO : ScriptableObject
{
    // 
    [SerializeField] protected StateType _type;

    public StateType Type => _type;

    public abstract State ProvideState();
}
