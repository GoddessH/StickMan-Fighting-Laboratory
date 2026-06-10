using Photon.Pun;
using UnityEngine;

public class CharacterSpawner : Singleton<CharacterSpawner>
{
    //
    private const string _characterName = "Player";

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        SpawnCharacter();
    }

    private void SpawnCharacter()
    {
        GameObject go = PhotonNetwork.Instantiate(_characterName, Vector3.zero, Quaternion.identity);
    }
}
