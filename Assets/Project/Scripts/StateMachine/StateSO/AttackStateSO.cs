using UnityEngine;

[CreateAssetMenu(fileName = "AttackStateSO", menuName = "ScriptableObject/StateSO/AttackStateSO")]
public class AttackStateSO : StateSO
{
    //
    public override State ProvideState()
        => new AttackState();
}
