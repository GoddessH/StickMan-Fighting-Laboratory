using Spine.Unity;
using UnityEngine;

public struct AttackAsset
{
    //
    private AnimationReferenceAsset _attack1;
    private AnimationReferenceAsset _attack2;
    private AnimationReferenceAsset _attack3;
    private AnimationReferenceAsset _attack4;

    public AnimationReferenceAsset Attack1 => _attack1;
    public AnimationReferenceAsset Attack2 => _attack2;
    public AnimationReferenceAsset Attack3 => _attack3;
    public AnimationReferenceAsset Attack4 => _attack4;
}
