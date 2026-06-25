using Spine.Unity;
using System;
using UnityEngine;

[Serializable]
public class AnimationAssetProgress
{
    //
    [SerializeField] private AnimationReferenceAsset _start;
    [SerializeField] private AnimationReferenceAsset _charge;
    [SerializeField] private AnimationReferenceAsset _end;

    public AnimationReferenceAsset Start => _start;
    public AnimationReferenceAsset Charge => _charge;
    public AnimationReferenceAsset End => _end;
}
