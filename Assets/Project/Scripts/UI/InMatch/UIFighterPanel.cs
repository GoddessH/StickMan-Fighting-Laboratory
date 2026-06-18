using Photon.Pun;
using TMPro;
using UnityEngine;

public class UIFighterPanel : MonoBehaviour
{
    //
    [SerializeField] private UIFighterHealthbar _characterHealthBar;
    [SerializeField] private UIFighterManabar _characterManabar;
    [SerializeField] private TextMeshProUGUI _characterName;

    public void ConnectFighterToFightPanel(string name, HealthManager healthManager, ManaManager manaManager)
    {
        _characterHealthBar?.ConnectToOwner(healthManager);
        _characterManabar?.ConnectToOwner(manaManager);

        if (_characterName != null) _characterName.text = name;
    }

    public void DisconnectFighterToFightPanel()
    {
        _characterHealthBar?.DisconnectToOwner();
        
        if (_characterName != null)
            _characterName.text = "";
    }
}
