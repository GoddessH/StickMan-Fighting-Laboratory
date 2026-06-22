using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [Header("Skills Configuration")]
    [SerializeField] private List<BaseSkill> _skills = new List<BaseSkill>();

    private Dictionary<SkillType, Skill> _activeSkills = new Dictionary<SkillType, Skill>();
    private List<SkillType> _configuredSkillTypes = new List<SkillType>();

    public void Init()
    {
        _activeSkills.Clear();
        _configuredSkillTypes.Clear();

        for (int i = 0; i < _skills.Count; i++)
        {
            BaseSkill config = _skills[i];
            if (config == null) continue;

            SkillType type = config.skillType;
            Skill skillInstance = CreateSkillInstance(config);
            skillInstance.Init(gameObject, config);

            _activeSkills[type] = skillInstance;

            if (!_configuredSkillTypes.Contains(type))
            {
                _configuredSkillTypes.Add(type);
            }
        }
    }

    private Skill CreateSkillInstance(BaseSkill config)
    {
        return config.CreateInstance();
    }

    private void Awake()
    {
        if (GetComponent<SkillCastingSupporter>() == null)
        {
            gameObject.AddComponent<SkillCastingSupporter>();
        }
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        for (int i = 0; i < _configuredSkillTypes.Count; i++)
        {
            SkillType key = _configuredSkillTypes[i];
            if (_activeSkills.TryGetValue(key, out var skill))
            {
                skill.UpdateCooldown(Time.deltaTime);

                if (skill.IsExecuting)
                {
                    skill.OnUpdate(Time.deltaTime);
                }
            }
        }
    }

    public BaseSkill GetSkillConfig(SkillType type)
    {
        if (_activeSkills.TryGetValue(type, out var skill))
            return skill.Config;
        return null;
    }

    public bool TryGetSkill(SkillType type, out Skill skill)
    {
        return _activeSkills.TryGetValue(type, out skill);
    }

    public bool IsOnCooldown(SkillType type)
    {
        if (_activeSkills.TryGetValue(type, out var skill))
        {
            return skill.CooldownTimer > 0f;
        }
        return false;
    }

    public bool CanCast(SkillType type)
    {
        if (_activeSkills.TryGetValue(type, out var skill))
        {
            return skill.CanCast();
        }
        return false;
    }

    public bool IsSkillExecuting(SkillType type)
    {
        if (_activeSkills.TryGetValue(type, out var skill))
        {
            return skill.IsExecuting;
        }
        return false;
    }

    public bool StartSkill(SkillType type)
    {
        if (_activeSkills.TryGetValue(type, out var skill))
        {
            if (skill.CanCast())
            {
                skill.Cast();
                return true;
            }
        }
        return false;
    }

    public void StopSkill(SkillType type)
    {
        if (_activeSkills.TryGetValue(type, out var skill))
        {
            skill.OnEnd();
        }
    }
}
