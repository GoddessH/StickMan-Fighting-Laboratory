using Photon.Pun;
using System;
using UnityEngine;

public class HurtController : MonoBehaviour, IProvider<Func<float>>, IDamageTakerEvent
{
    //
    [SerializeField] private HurtPoint _hurtPoint;

    private PhotonView _photonView;
    private Action _onTakeDamage;
    private IStateRequestReceiver _requestReceiver;
    private float _cachedLastDamages;

    #region Supporter
    private StateRequester _hurtRequester;
    #endregion

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _requestReceiver = GetComponent<IStateRequestReceiver>();
        _hurtRequester = new StateRequester(StateType.Hurt);
        _hurtPoint?.Init(Hurt);
    }

    [PunRPC]
    private void RPCRequestHurt() => _hurtRequester?.RequestState(_requestReceiver);

    private void Hurt(float damages)
    {
        _cachedLastDamages = damages;
        _onTakeDamage?.Invoke();

        if (_photonView == null) RPCRequestHurt();
        else _photonView.RPC(nameof(RPCRequestHurt), RpcTarget.All);
    }

    #region Implement IProvider
    /// <summary>
    /// Provide cachedLastDamages getter
    /// </summary>
    public Func<float> Provide()
        => () => _cachedLastDamages;
    #endregion

    #region Explicit implement IDamageTakerEvent
    void IDamageTakerEvent.SubscribeTakeDamageEvent(Action subscriber)
    {
        _onTakeDamage -= subscriber;
        _onTakeDamage += subscriber;
    }
    void IDamageTakerEvent.UnsubscribeTakeDamageEvent(Action unSubscriber)
    {
        _onTakeDamage -= unSubscriber;
    }
    #endregion
}
