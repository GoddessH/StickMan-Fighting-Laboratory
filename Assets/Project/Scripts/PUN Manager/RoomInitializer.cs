using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInitializer
{
    //
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private SpawnerManager _spawnerManagerPrefab;
    [SerializeField] private UIInMatchManager _uiInMatchManager;

    private bool CheckOwnerClient(Character fighter)
        => fighter != null && fighter.photonView.IsMine && fighter is not BotController;

    private void SetupFighterFacing(Character fighterA, Character fighterB)
    {
        fighterA?.VisualRoot?.RotationHandler?.SetTarget(fighterB?.transform);
        fighterB?.VisualRoot?.RotationHandler?.SetTarget(fighterA?.transform);
    }

    private void SetupFighterIndicator(Character fighterA, Character fighterB)
    {
        if (CheckOwnerClient(fighterA)) fighterA?.VisualRoot?.IndicatorRoot?.SetupIndicator(fighterB?.VisualRoot?.IndicatorRoot);
        if (CheckOwnerClient(fighterB)) fighterB?.VisualRoot?.IndicatorRoot?.SetupIndicator(fighterA?.VisualRoot?.IndicatorRoot);
    }

    private void SetupCamera(Character fighterA, Character fighterB)
    {
        if (_cameraManager == null) return;

        if (CheckOwnerClient(fighterA)) _cameraManager.Setup(fighterA.transform, fighterB.transform);
        if (CheckOwnerClient(fighterB)) _cameraManager.Setup(fighterB.transform, fighterA.transform);
    }

    public void SetupMatch()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (_spawnerManagerPrefab != null) PhotonNetwork.InstantiateRoomObject(_spawnerManagerPrefab.name, Vector3.zero, Quaternion.identity);
    }

    /// <param name="fighterStack">The caller should pass the reference of a copy, not the original</param>
    public void SetupFighterNeededData(Stack<Character> fighterStack)
    {
        if (fighterStack == null || fighterStack.Count != 2) return;

        Character secondFighter = fighterStack.Pop();
        Character firstFighter = fighterStack.Pop();

        SetupCamera(secondFighter, firstFighter);
        SetupFighterFacing(secondFighter, firstFighter);
        SetupFighterIndicator(secondFighter, firstFighter);
    }

    public void SetupUI(Stack<Character> fighterStack)
    {
        _uiInMatchManager?.ConnectFighterToUI(fighterStack);
    }
}
