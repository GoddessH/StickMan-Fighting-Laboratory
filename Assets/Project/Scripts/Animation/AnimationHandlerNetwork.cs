using Photon.Pun;
using Spine.Unity;

public class AnimationHandlerNetwork : AnimationHandle
{
    // 
    private PhotonView _photonView;

    private PhotonView PhotonView => _photonView == null ? _photonView = GetComponent<PhotonView>() : _photonView;


    [PunRPC]
    private void RPCSetAnimation(string animationName, bool isLoop) => SetAnimationLocal(animationName, isLoop);

    #region Override AnimationHandler
    public override void SetAnimation(AnimationReferenceAsset animation, bool isLoop)
    {
        if (animation == null) return;

        if (PhotonView == null) SetAnimationLocal(animation.name, isLoop);
        else PhotonView.RPC(nameof(RPCSetAnimation), RpcTarget.All, animation.name, isLoop);
    }
    #endregion
}
