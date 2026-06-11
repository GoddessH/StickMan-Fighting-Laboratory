using UnityEngine;
using UnityEngine.UI;

public class UIFighterManabar : MonoBehaviour
{
    //
    [SerializeField] private Image _manaFill;

    private ManaManager _ownerManaManager;

    private void Awake()
    {
        _manaFill.fillAmount = .5f;
    }

    private void OnUpdateManaFill(float currentMana, float maxMana)
    {
        if (_manaFill == null) return;

        float fillAmount = currentMana / maxMana;
        _manaFill.fillAmount = fillAmount;
    }

    public void ConnectToOwner(ManaManager manaManager)
    {
        if (manaManager == null) return;
        _ownerManaManager = manaManager;
        _ownerManaManager.OnChangeMana += OnUpdateManaFill;
    }

    public void DisconnectToOwner()
    {
        if (_ownerManaManager == null) return;

        _ownerManaManager.OnChangeMana -= OnUpdateManaFill;
    }
}
