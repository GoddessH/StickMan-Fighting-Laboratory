using UnityEngine;
using Photon.Pun;

public class RoomRegister : MonoBehaviourPun
{
    // 
    private Character _owner;

    public void RegistRoom(Character owner)
    {
        _owner = owner;
        RPCRegistRoom();
    }

    [PunRPC]
    private void RPCRegistRoom()
        => RoomManager.Instance.AddPlayer(_owner);
}
