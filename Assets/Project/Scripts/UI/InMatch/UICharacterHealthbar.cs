using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterHealthbar : MonoBehaviour
{
    //
    [SerializeField] private HealthManager _ownerHealthManager;
    [SerializeField] private Image _healthFill;
    [SerializeField] private Image _healthTrail;
    [SerializeField] private float _changeHealthDuration;
    [SerializeField] private float _trailDelayTime;

    private void Awake()
    {

        _healthFill.fillAmount = 1;
        _healthTrail.fillAmount = 1;
    }

    private void OnEnable()
    {
        if (_ownerHealthManager == null) return;
        _ownerHealthManager.OnChangeHealth += OnUpdateHealthFill;
    }
    private void OnDisable()
    {
        if (_ownerHealthManager == null) return;
        _ownerHealthManager.OnChangeHealth -= OnUpdateHealthFill;
    }

    private void OnDestroy()
    {
        _healthFill.DOKill();
        _healthTrail.DOKill();
    }

    private void OnUpdateHealthFill(float currentHealth, float maxHealth)
    {
        if (_healthFill == null) return;

        float fillAmount = currentHealth / maxHealth;

        Sequence healthFillSequence = DOTween.Sequence();
        healthFillSequence.SetAutoKill(true);
        healthFillSequence.SetLink(gameObject);

        healthFillSequence.Append(_healthFill.DOFillAmount(fillAmount, _changeHealthDuration).SetEase(Ease.OutQuart));
        healthFillSequence.AppendInterval(_trailDelayTime);
        healthFillSequence.Append(_healthTrail.DOFillAmount(fillAmount, _changeHealthDuration).SetEase(Ease.OutQuart));
        healthFillSequence.Play();
    }

    public void SetHealthManager()
    {

    }
}
