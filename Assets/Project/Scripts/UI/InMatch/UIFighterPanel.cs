using Photon.Pun;
using TMPro;
using UnityEngine;

public class UIFighterPanel : MonoBehaviour
{
    //
    [SerializeField] private UIFighterHealthbar _characterHealthBar;
    [SerializeField] private TextMeshProUGUI _characterName;

    public void ConnectToFighter(string name, HealthManager healthManager)
    {
        _characterHealthBar?.ConnectToOwner(healthManager);

        if (_characterName != null) _characterName.text = name;
    }

    public void DisconnectToFighter()
    {
        _characterHealthBar?.DisconnectToOwner();
        
        if (_characterName != null)
            _characterName.text = "";
    }
}
