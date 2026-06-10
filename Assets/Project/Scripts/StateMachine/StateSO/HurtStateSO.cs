using UnityEngine;

[CreateAssetMenu(fileName = "HurtStateSO", menuName = "ScriptableObject/StateSO/HurtStateSO")]
public class HurtStateSO : StateSO
{
    //
    public override State ProvideState()
        => new HurtState();
}
