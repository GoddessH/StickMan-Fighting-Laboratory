using UnityEngine;

[CreateAssetMenu(fileName = "IdleStateSO", menuName = "ScriptableObject/StateSO/IdleStateSO")]
public class IdleStateSO : StateSO
{
    //
    public override State ProvideState()
        => new IdleState();
}
