using Spine;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(AnimationAssetLibrary))]
public class AnimationHandle : MonoBehaviour
{
    // 
    [SerializeField] protected SkeletonAnimation _skeletonAnimation;
    protected AnimationAssetLibrary _library;

    public AnimationAssetLibrary Library => _library = ComponentEnsurer.EnsureComponent(GetComponent<AnimationAssetLibrary>(), gameObject);

    public TrackEntry SetAnimation(AnimationReferenceAsset animation, bool isLoop)
    {
        if (_skeletonAnimation == null || animation == null) return null;

        return _skeletonAnimation.AnimationState.SetAnimation(0, animation, isLoop);
    }

    public bool IsCurrentAnimationName(AnimationReferenceAsset animation)
    {
        if (_skeletonAnimation == null || animation == null) return false;

        return _skeletonAnimation.AnimationName == animation.name;
    }

    public TrackEntry GetCurrentTrack() => _skeletonAnimation.AnimationState.GetCurrent(0);
}
