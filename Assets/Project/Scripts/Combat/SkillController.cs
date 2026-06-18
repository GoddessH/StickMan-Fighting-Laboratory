using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [Header("Skills Configuration")]
    [SerializeField] private List<BaseSkill> _skills = new List<BaseSkill>();

    private IManaConsumer _manaConsumer;
    private IManaChecker _manaChecker;
    private CharacterInput _characterInput;

    private Dictionary<SkillType, Skill> _activeSkills = new Dictionary<SkillType, Skill>();
    private List<SkillType> _configuredSkillTypes = new List<SkillType>();

    private HashSet<SkillType> _activeContinuousSkills = new HashSet<SkillType>();
    private Dictionary<SkillType, float> _continuousAccumulators = new Dictionary<SkillType, float>();

    public void Init(IManaConsumer manaConsumer, IManaChecker manaChecker)
    {
        _manaConsumer = manaConsumer;
        _manaChecker = manaChecker;
        
        _activeSkills.Clear();
        _configuredSkillTypes.Clear();
        _continuousAccumulators.Clear();

        for (int i = 0; i < _skills.Count; i++)
        {
            BaseSkill config = _skills[i];
            if (config == null) continue;

            SkillType type = config.skillType;
            Skill skillInstance = CreateSkillInstance(config);
            skillInstance.Init(gameObject, config);

            _activeSkills[type] = skillInstance;
            _continuousAccumulators[type] = 0f;

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

    private void Start()
    {
        _characterInput = GetComponent<CharacterInput>();
        IManaConsumer mana = GetComponent<IManaConsumer>();
        IManaChecker manaChecker = GetComponent<IManaChecker>();
        if (mana != null || manaChecker != null)
        {
            Init(mana, manaChecker);
        }
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
        if (_characterInput == null) _characterInput = GetComponent<CharacterInput>();
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

    private void RequestSkill(SkillType type)
    {
        if (CanCast(type))
        {
            StartSkill(type);
        }
    }

    private EventInput GetInputForSlot(SkillType type)
    {
        if (_characterInput == null) return null;
        switch (type)
        {
            case SkillType.Block:
                return _characterInput.BlockInput;
            case SkillType.Flash:
                return _characterInput.FlashInput;
            default:
                if (type >= SkillType.Skill1 && type <= SkillType.Skill4)
                {
                    int index = type - SkillType.Skill1;
                    var inputs = _characterInput.SkillInputs;
                    if (inputs != null && index >= 0 && index < inputs.Count)
                    {
                        return inputs[index];
                    }
                }
                return null;
        }
    }

    private void Update()
    {
        // 1. Cập nhật cooldowns và tự động tick active skills không GC Alloc
        for (int i = 0; i < _configuredSkillTypes.Count; i++)
        {
            SkillType key = _configuredSkillTypes[i];
            if (_activeSkills.TryGetValue(key, out var skill))
            {
                skill.UpdateCooldown(Time.deltaTime);

                if (skill.IsExecuting)
                {
                    skill.OnUpdate(Time.deltaTime);

                    if (skill.Config.isContinuous)
                    {
                        bool isHeld = false;
                        var input = GetInputForSlot(key);
                        if (input != null && input.Provide() != null)
                        {
                            isHeld = input.Provide().Invoke();
                        }

                        if (!isHeld)
                        {
                            StopSkill(key);
                        }
                        else
                        {
                            _activeContinuousSkills.Add(key);
                            if (!ConsumeContinuousMana(key, Time.deltaTime))
                            {
                                StopSkill(key);
                            }
                        }
                    }
                }
            }
        }

        // 2. Reset bộ tích lũy duy trì không GC Alloc
        for (int i = 0; i < _configuredSkillTypes.Count; i++)
        {
            SkillType key = _configuredSkillTypes[i];
            if (!_activeContinuousSkills.Contains(key))
            {
                _continuousAccumulators[key] = 0f;
            }
        }

        _activeContinuousSkills.Clear();
    }

    public BaseSkill GetSkillConfig(SkillType type)
    {
        if (_activeSkills.TryGetValue(type, out var skill))
            return skill.Config;
        return null;
    }

    public bool HasEnoughMana(float amount)
    {
        return _manaChecker != null && _manaChecker.HasManaReached(amount);
    }

    public void ConsumeMana(float amount)
    {
        _manaConsumer?.ConsumeMana(amount);
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
            return skill.CanCast(this);
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
            if (skill.CanCast(this))
            {
                skill.Cast(this);
                if (skill.Config.isContinuous)
                {
                    _continuousAccumulators[type] = 0f;
                    _activeContinuousSkills.Add(type);
                }
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
            _activeContinuousSkills.Remove(type);
            _continuousAccumulators[type] = 0f;
        }
    }

    private bool ConsumeContinuousMana(SkillType type, float deltaTime)
    {
        BaseSkill config = GetSkillConfig(type);
        if (config == null || !config.isContinuous)
            return false;

        _continuousAccumulators[type] += deltaTime;

        if (_continuousAccumulators[type] >= 1.0f)
        {
            if (HasEnoughMana(config.manaCost))
            {
                ConsumeMana(config.manaCost);
                _continuousAccumulators[type] -= 1.0f;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}
