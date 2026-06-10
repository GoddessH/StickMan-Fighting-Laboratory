using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]
public class AnimationHandler : MonoBehaviourPun
{
    // 
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    [PunRPC]
    private void RPCSetBool(string flag, bool value)
    {
        _animator.SetBool(flag, value);
    }

    [PunRPC]
    private void RPCSetInteger(string parameter, int value)
    {
        _animator.SetInteger(parameter, value);
    }

    public bool GetBool(string flag)
        => _animator.GetBool(flag);

    public void SetBool(string flag, bool value)
    {
        if (!photonView.IsMine) return;
        photonView.RPC(nameof(RPCSetBool), RpcTarget.All, flag, value);
    }

    public void SetInteger(string parameter, int value)
    {
        if (!photonView.IsMine) return;
        photonView.RPC(nameof(RPCSetInteger), RpcTarget.All, parameter, value);
    }

    public (bool, float) CheckCurrentState(string stateName)
    {
        AnimatorStateInfo currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return (currentStateInfo.IsName(stateName), currentStateInfo.normalizedTime);
    }
}
