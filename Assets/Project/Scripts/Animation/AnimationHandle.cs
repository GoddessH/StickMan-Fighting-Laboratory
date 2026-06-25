using Spine;
using Spine.Unity;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(AnimationAssetLibrary))]
public class AnimationHandle : MonoBehaviour
{
    // 
    [SerializeField] protected SkeletonAnimation _skeletonAnimation;
    protected AnimationAssetLibrary _library;

    public AnimationAssetLibrary Library => _library = ComponentEnsurer.EnsureComponent(GetComponent<AnimationAssetLibrary>(), gameObject);

    protected void SetAnimationLocal(string animationName, bool isLoop)
    {
        if (_skeletonAnimation == null) return;

        _skeletonAnimation.AnimationState.SetAnimation(0, animationName, isLoop);
    }

    public virtual void SetAnimation(AnimationReferenceAsset animation, bool isLoop)
    {
        if (animation == null) return;
        SetAnimationLocal(animation.name, isLoop);
    }

    public bool IsCurrentAnimationName(string name)
    {
        if (_skeletonAnimation == null) return false;

        return _skeletonAnimation.AnimationName == name;
    }

    public TrackEntry GetCurrentTrack() => _skeletonAnimation.AnimationState.GetCurrent(0);
}
