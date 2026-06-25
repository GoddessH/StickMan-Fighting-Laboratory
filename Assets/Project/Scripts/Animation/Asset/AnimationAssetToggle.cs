using Spine.Unity;
using System;
using UnityEngine;

[Serializable]
public class AnimationAssetToggle
{
    //
    [SerializeField] private AnimationReferenceAsset _primaryAnimation;
    [SerializeField] private AnimationReferenceAsset _seconđaryAnimation;

    public AnimationReferenceAsset PrimaryAnimation => _primaryAnimation;
    public AnimationReferenceAsset SecondaryAnimation => _seconđaryAnimation;
}
