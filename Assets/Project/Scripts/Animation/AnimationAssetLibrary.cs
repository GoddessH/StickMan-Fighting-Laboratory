using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAssetLibrary : MonoBehaviour
{
    //
    [SerializeField] private List<AnimationReferenceAsset> _attackList = new List<AnimationReferenceAsset>();

    [SerializeField] private AnimationReferenceAsset _buff;
    [SerializeField] private AnimationReferenceAsset _changeForm;
    [SerializeField] private AnimationReferenceAsset _dead;
    [SerializeField] private AnimationReferenceAsset _defense;
    [SerializeField] private List<AnimationReferenceAsset> _hitList = new List<AnimationReferenceAsset>();
    [SerializeField] private (AnimationReferenceAsset idleInAir, AnimationReferenceAsset idleOnGround) _idle;

}
