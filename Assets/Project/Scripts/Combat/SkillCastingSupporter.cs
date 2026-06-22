using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(SkillController))]
[RequireComponent(typeof(CharacterInput))]
public class SkillCastingSupporter : MonoBehaviour
{
    private SkillController _skillController;
    private CharacterInput _characterInput;
    private IManaChecker _manaChecker;

    private void Awake()
    {
        _skillController = GetComponent<SkillController>();
        _characterInput = GetComponent<CharacterInput>();
        _manaChecker = GetComponent<IManaChecker>();
    }

    private void OnEnable()
    {
        SubscribeInputs();
    }

    private void OnDisable()
    {
        UnsubscribeInputs();
    }

    private void SubscribeInputs()
    {
        if (_characterInput != null)
        {
            var inputs = _characterInput.SkillInputs;
            if (inputs != null)
            {
                for (int i = 0; i < inputs.Count; i++)
                {
                    int index = i;
                    SkillType type = (SkillType)((int)SkillType.Skill1 + index);
                    inputs[index]?.SubscribeInputAction(() => RequestSkill(type));
                }
            }
        }
    }

    private void UnsubscribeInputs()
    {
        if (_characterInput != null)
        {
            var inputs = _characterInput.SkillInputs;
            if (inputs != null)
            {
                for (int i = 0; i < inputs.Count; i++)
                {
                    inputs[i]?.UnsubscribeInputAction();
                }
            }
        }
    }

    public void RequestSkill(SkillType type)
    {
        if (_skillController != null && _skillController.TryGetSkill(type, out Skill skill))
        {
            if (skill != null && skill.CanCast())
            {
                skill.Cast();
            }
        }
    }

    public bool CanCast(SkillType type)
    {
        if (_skillController != null && _skillController.TryGetSkill(type, out Skill skill))
        {
            return skill != null && skill.CanCast();
        }
        return false;
    }

    public void CastSkill(SkillType type)
    {
        if (_skillController != null && _skillController.TryGetSkill(type, out Skill skill))
        {
            if (skill != null && skill.CanCast())
            {
                skill.Cast();
            }
        }
    }
}
