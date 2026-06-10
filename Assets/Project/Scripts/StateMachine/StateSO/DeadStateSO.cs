using UnityEngine;

[CreateAssetMenu(fileName = "DeadStateSO", menuName = "ScriptableObject/StateSO/DeadStateSO")]
public class DeadStateSO : StateSO
{
    //
    public override State ProvideState()
        => new DeadState();
}
