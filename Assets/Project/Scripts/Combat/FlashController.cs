using System;
using UnityEngine;

[RequireComponent(typeof(CharacterInput))]
public class FlashController : MonoBehaviour, IProvider<Func<bool>>
{
    private SkillController _skillController;
    private EventInput _flashInput;
    private IManaChecker _manaChecker;

    private void Awake()
    {
        _skillController = GetComponent<SkillController>();
        _flashInput = GetComponent<CharacterInput>().FlashInput;
        _manaChecker = GetComponent<IManaChecker>();
    }

    private void OnEnable()
    {
        if (_flashInput != null)
        {
            _flashInput.SubscribeInputAction(RequestFlash);
        }
    }

    private void OnDisable()
    {
        if (_flashInput != null)
        {
            _flashInput.UnsubscribeInputAction();
        }
    }

    private void RequestFlash()
    {
        if (_skillController != null && _manaChecker != null)
        {
            var config = _skillController.GetSkillConfig(SkillType.Flash);
            if (config != null)
            {
                if (!_manaChecker.HasManaReached(config.manaCost)) return;
                if (_skillController.IsOnCooldown(SkillType.Flash)) return;
            }
            _skillController.StartSkill(SkillType.Flash);
        }
    }

    #region Implement IProvider
    public Func<bool> Provide()
        => _flashInput?.Provide();
    #endregion
}
