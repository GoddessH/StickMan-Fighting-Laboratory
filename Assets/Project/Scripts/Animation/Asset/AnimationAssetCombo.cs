using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationAssetCombo
{
    //
    [SerializeField] private List<AnimationReferenceAsset> _animationList = new List<AnimationReferenceAsset>();

    public AnimationReferenceAsset this[int index]
    {
        get
        {
            if (_animationList.Count < 1 || !_animationList.IsContainIndex(index)) return null;

            return _animationList[index];
        }
    }
}
