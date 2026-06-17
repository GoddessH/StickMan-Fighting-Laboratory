using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInitializer
{
    //
    [SerializeField] private SpawnerManager _spawnerManagerPrefab;
    [SerializeField] private UIInMatchManager _uiInMatchManager;

    public void SetupMatch()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (_spawnerManagerPrefab != null) PhotonNetwork.InstantiateRoomObject(_spawnerManagerPrefab.name, Vector3.zero, Quaternion.identity);
    }

    /// <param name="fighterStack">The caller should pass the reference of a copy, not the original</param>
    public void InitFighterFacing(Stack<Character> fighterStack)
    {
        if (fighterStack.Count == 2)
        {
            Character fighterA = fighterStack.Pop();
            Character fighterB = fighterStack.Pop();
            fighterA?.VisualRoot?.RotationHandler?.SetTarget(fighterB?.transform);
            fighterB?.VisualRoot?.RotationHandler?.SetTarget(fighterA?.transform);
        }
    }

    public void SetupUI(Stack<Character> fighterStack)
    {
        _uiInMatchManager?.ConnectToFighter(fighterStack);
    }
}
