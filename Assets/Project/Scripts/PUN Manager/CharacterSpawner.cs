using Photon.Pun;
using UnityEngine;

public class CharacterSpawner : Singleton<CharacterSpawner>
{
    //
    private const string _characterName = "Player";

    private void Start()
    {
        SpawnCharacter();
    }

    private void SpawnCharacter()
    {
        GameObject go = PhotonNetwork.Instantiate(_characterName, Vector3.zero, Quaternion.identity);
    }
}
