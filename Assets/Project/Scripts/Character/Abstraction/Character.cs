using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(StateController))]
public class Character : MonoBehaviourPun/*, IPunObservable*/
{
    //
    private StateController _stateController;
    //private Vector2 _networkedPosition;

    //private float _lerpRate = .1f;


    private void Awake()
    {
        _stateController = GetComponent<StateController>();
    }

    private void Start()
    {
        //photonView.RPC(nameof(RegistRoom), RpcTarget.AllViaServer);
        _stateController.Init(gameObject);
    }


    [PunRPC]
    private void RegistRoom()
    {
        TempGameMaster.Instance.AddPlayer(this);
    }
}
