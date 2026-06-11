using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class JoinRoomHandler : MonoBehaviourPunCallbacks
{
    //
    private Action _nextSceneLoader;

    public void ConnectToMasterServer(Action onLoadNextScene)
    {
        _nextSceneLoader = onLoadNextScene;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, TypedLobby.Default, null, null, new RoomOptions { MaxPlayers = 2 });
        Debug.LogWarning("Connected!");
    }

    public override void OnJoinedRoom()
    {
        _nextSceneLoader?.Invoke();
    }
}
