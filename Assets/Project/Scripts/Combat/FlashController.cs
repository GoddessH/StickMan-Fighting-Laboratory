using System;
using UnityEngine;

[RequireComponent(typeof(CharacterInput))]
public class FlashController : MonoBehaviour, IProvider<Func<bool>>
{
    private SkillController _skillController;
    private FlashInput _flashInput;

    private void Awake()
    {
        _skillController = GetComponent<SkillController>();
        _flashInput = GetComponent<CharacterInput>().FlashInput;
    }

    private void OnEnable()
    {
        if (_flashInput != null)
        {
            _flashInput.SubscribeFlashAction(RequestFlash);
        }
    }

    private void OnDisable()
    {
        if (_flashInput != null)
        {
            _flashInput.UnsubscribeFlashAction();
        }
    }

    private void RequestFlash()
    {
        if (_skillController != null && _skillController.CanCast(SkillType.Flash))
        {
            _skillController.StartSkill(SkillType.Flash);
        }
    }

    #region Implement IProvider
    public Func<bool> Provide()
        => _flashInput?.Provide();
    #endregion
}
