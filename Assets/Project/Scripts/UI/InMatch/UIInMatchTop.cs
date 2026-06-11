using System.Collections.Generic;
using UnityEngine;

public class UIInMatchTop : MonoBehaviour
{
    //
    private const int _MAX_FIGHTER_PANEL = 2;

    [SerializeField] UIFighterPanel[] _fighterPanels = new UIFighterPanel[_MAX_FIGHTER_PANEL];

    private Dictionary<Character, UIFighterPanel> _connectedFighterDictionary = new Dictionary<Character, UIFighterPanel>();


    /// <returns>Return true if the next logic should not be executed</returns>
    private bool GuardCheck(Character fighter)
    {
        return (_connectedFighterDictionary.Count > 0 && _connectedFighterDictionary.ContainsKey(fighter)) || _fighterPanels.Length > _MAX_FIGHTER_PANEL;
    }

    public void ConnectFighterToValidPanel(Character fighter)
    {
        if (GuardCheck(fighter)) return;

        foreach (var panel in _fighterPanels)
        {
            if (panel == null || _connectedFighterDictionary.ContainsValue(panel)) continue;

            panel.ConnectToFighter(fighter.gameObject.name, fighter.GetComponent<HealthManager>());
            _connectedFighterDictionary[fighter] = panel;
            break;
        }
    }

    public void DisconnectFighter(Character fighter)
    {
        if (GuardCheck(fighter)) return;

        _connectedFighterDictionary[fighter].DisconnectToFighter();
        _connectedFighterDictionary.Remove(fighter);
    }
}
