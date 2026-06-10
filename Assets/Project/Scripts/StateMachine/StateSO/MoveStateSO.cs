using UnityEngine;

[CreateAssetMenu(fileName = "MoveStateSO", menuName = "ScriptableObject/StateSO/MoveStateSO")]
public class MoveStateSO : StateSO
{
    // 
    public override State ProvideState()
        => new MoveState();
}
