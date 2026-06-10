using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class JoinRoomHandler : MonoBehaviourPunCallbacks
{
    //
    [SerializeField] private NextSceneLoader _nextSceneLoader;

    public void ConnectToMasterServer()
        => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Test_room", new RoomOptions { MaxPlayers = 2}, TypedLobby.Default);
        Debug.LogWarning("Connected!");
    }

    public override void OnJoinedRoom()
    {
        _nextSceneLoader.LoadNextScene();
    }
}
