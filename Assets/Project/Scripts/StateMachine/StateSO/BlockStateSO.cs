using UnityEngine;

[CreateAssetMenu(fileName = "BlockStateSO", menuName = "ScriptableObject/StateSO/BlockStateSO")]
public class BlockStateSO : StateSO
{
    //
    public override State ProvideState()
        => new BlockState();
}
