using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class UIInMatchTop : MonoBehaviour
{
    //
    private const int _MAX_FIGHTER_PANEL = 2;

    [SerializeField] UIFighterPanel[] _fighterPanels = new UIFighterPanel[_MAX_FIGHTER_PANEL];

    /// <returns>Return true if the next logic should not be executed</returns>
    private bool GuardCheck()
    {
        return _fighterPanels.Length > _MAX_FIGHTER_PANEL;
    }

    public void SetupFighter(Stack<Character> fighterStack)
    {
        if (GuardCheck()) return;
        
        foreach(var fighter in fighterStack)
        {
            int panelIndex = 0;
            if (fighter.photonView.OwnerActorNr != PhotonNetwork.CurrentRoom.MasterClientId)
            {
                fighter.gameObject.name += "2";
                panelIndex = 1;
            }
            else fighter.gameObject.name += "1";

            _fighterPanels[panelIndex].ConnectToFighter(fighter.gameObject.name, fighter.GetComponent<HealthManager>());
            //Debug.Log($"{fighter.photonView.ViewID}: {_fighterPanels[panelIndex].gameObject.name}");
        }
    }

    public void DisconnectAllFighter()
    {
        foreach (var panel in _fighterPanels) panel.DisconnectToFighter();
    }
}
