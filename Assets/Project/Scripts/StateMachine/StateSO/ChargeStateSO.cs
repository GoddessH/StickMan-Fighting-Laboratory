using UnityEngine;

[CreateAssetMenu(fileName = "ChargeStateSO", menuName = "ScriptableObject/StateSO/ChargeStateSO")]
public class ChargeStateSO : StateSO
{
    //
    public override State ProvideState()
        => new ChargeState();
}
