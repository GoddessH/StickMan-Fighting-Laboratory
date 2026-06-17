using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(StateController))]
public class Character : MonoBehaviourPun
{
    //
    [SerializeField] private CharacterVisualRoot _visualRoot;

    private StateController _stateController;

    public CharacterVisualRoot VisualRoot => _visualRoot;

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
