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


    private void SetupFighterFacing(Character fighterA, Character fighterB)
    {
        fighterA?.VisualRoot?.RotationHandler?.SetTarget(fighterB?.transform);
        fighterB?.VisualRoot?.RotationHandler?.SetTarget(fighterA?.transform);
    }

    private void SetupFighterIndicator(Character fighterA, Character fighterB)
    {
        fighterA?.VisualRoot?.IndicatorRoot?.SetupIndicator(fighterB?.VisualRoot?.IndicatorRoot);
        fighterB?.VisualRoot?.IndicatorRoot?.SetupIndicator(fighterA?.VisualRoot?.IndicatorRoot);
    }

    public void SetupMatch()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (_spawnerManagerPrefab != null) PhotonNetwork.InstantiateRoomObject(_spawnerManagerPrefab.name, Vector3.zero, Quaternion.identity);
    }

    /// <param name="fighterStack">The caller should pass the reference of a copy, not the original</param>
    public void SetupFighterNeededData(Stack<Character> fighterStack)
    {
        if (fighterStack.Count == 2)
        {
            Character secondFighter = fighterStack.Pop();
            Character firstFighter = fighterStack.Pop();

            SetupFighterFacing(secondFighter, firstFighter);
            SetupFighterIndicator(secondFighter, firstFighter);
        }
    }

    public void SetupUI(Stack<Character> fighterStack)
    {
        _uiInMatchManager?.ConnectToFighter(fighterStack);
    }
}
