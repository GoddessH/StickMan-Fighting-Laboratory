using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CharacterIndicatorRoot : MonoBehaviour, IEnemyIndicator
{
    //
    [SerializeField] private Canvas _indicatorCanvas;
    [SerializeField] private Image _indicatorImage;
    [SerializeField] private Sprite[] _indicatorSprites = new Sprite[2];

    public void SetupIndicator(IEnemyIndicator enemyIndicator)
    {
        if (_indicatorCanvas != null) _indicatorCanvas.worldCamera = Camera.main;

        if (_indicatorImage == null || _indicatorSprites == null || _indicatorSprites.Length < 1 || _indicatorSprites.Length >= 3) return;

        _indicatorImage.sprite = _indicatorSprites[0];
        enemyIndicator?.SetEnemyIndicator(_indicatorSprites[1]);
    }

    #region Explicit implement IEnemyIndicator
    void IEnemyIndicator.SetEnemyIndicator(Sprite sprite)
        => _indicatorImage.sprite = sprite;
    #endregion
}
