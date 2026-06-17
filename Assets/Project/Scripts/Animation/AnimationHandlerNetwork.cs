using Photon.Pun;

public class AnimationHandlerNetwork : AnimationHandler
{
    //
    private PhotonView _photonView;

    [PunRPC]
    private void RPCSetBool(string flag, bool value)
        => LocalSetBool(flag, value);

    [PunRPC]
    private void RPCSetInteger(string parameter, int value)
        => LocalSetInteger(parameter, value);

    #region Override AnimationHandler
    protected void Awake()
    {
        _photonView = ComponentEnsurer.EnsureComponent(GetComponent<PhotonView>(), gameObject);
    }

    public override void SetBool(string flag, bool value)
    {
        if (!_photonView.IsMine) return;

        _photonView.RPC(nameof(RPCSetBool), RpcTarget.All, flag, value);
    }

    public override void SetInteger(string parameter, int value)
    {
        if (!_photonView.IsMine) return;

        _photonView.RPC(nameof(RPCSetInteger), RpcTarget.All, parameter, value);
    }
    #endregion
}
