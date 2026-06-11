using Photon.Pun;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    //
    private const string _characterName = "Bot";

    public void SpawnCharacter()
    {
        GameObject go = PhotonNetwork.InstantiateRoomObject(_characterName, Vector3.zero, Quaternion.identity);
    }
}
