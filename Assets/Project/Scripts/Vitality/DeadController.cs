using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DeadController : MonoBehaviour, IProvider<Action<bool>>
{
    //
    [SerializeField] private List<Behaviour> _disableCompnentList = new List<Behaviour>();

    private PhotonView _photonView;
    private IStateRequestReceiver _requestReceiver;

    #region Supporter
    private StateRequester _deadRequester;
    #endregion

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _requestReceiver = GetComponent<IStateRequestReceiver>();
        _deadRequester = new StateRequester(StateType.Dead);
    }

    private void SetEnableHurtPoint(bool flag)
    {
        if (_photonView == null) RPCSetActionHurtPoint(flag);
        else _photonView.RPC(nameof(RPCSetActionHurtPoint), RpcTarget.All, flag);
    }

    [PunRPC]
    private void RPCSetActionHurtPoint(bool flag)
    {
        foreach (var component in _disableCompnentList) component.enabled = flag;
    }

    public void RequestDeadState()
    {
        if (_requestReceiver == null) return;

        _deadRequester.RequestState(_requestReceiver);
    }

    #region Implement IProvider
    /// <summary>
    /// Provide to DeadState
    /// </summary>
    public Action<bool> Provide()
        => SetEnableHurtPoint;
    #endregion
}
