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
        
        RoomRegister roomRegister = GetComponent<RoomRegister>();
        if (roomRegister != null) roomRegister.RegistRoom(this);
    }

    private void Start()
    {
        _stateController.Init(gameObject);
    }
}
