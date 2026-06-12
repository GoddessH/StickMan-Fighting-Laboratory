using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [Header("Skills Configuration")]
    [SerializeField] private List<BaseSkill> _skills = new List<BaseSkill>();

    private IManaConsumer _manaConsumer;
    private IManaState _manaState;
    private Dictionary<SkillType, float> _cooldownTimers = new Dictionary<SkillType, float>();

    private List<SkillType> _activeContinuousSkills = new List<SkillType>();
    private Dictionary<SkillType, float> _continuousAccumulators = new Dictionary<SkillType, float>();
    private float _statusLogTimer;

    public void Init(IManaConsumer manaConsumer, IManaState manaState)
    {
        _manaConsumer = manaConsumer;
        _manaState = manaState;
        for (int i = 0; i < _skills.Count; i++)
        {
            if (_skills[i] != null)
                _cooldownTimers[_skills[i].skillType] = 0f;
        }
    }

    private void Start()
    {
        IManaConsumer mana = GetComponent<IManaConsumer>();
        IManaState manaState = GetComponent<IManaState>();
        if (mana != null || manaState != null)
        {
            Init(mana, manaState);
        }
    }

    private void Update()
    {
        // Cập nhật Cooldowns
        List<SkillType> keys = new List<SkillType>(_cooldownTimers.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            SkillType key = keys[i];
            if (_cooldownTimers[key] > 0)
                _cooldownTimers[key] -= Time.deltaTime;
        }

        // Reset bộ tích lũy thời gian của các skill duy trì không hoạt động ở frame trước
        List<SkillType> accumulatorKeys = new List<SkillType>(_continuousAccumulators.Keys);
        for (int i = 0; i < accumulatorKeys.Count; i++)
        {
            SkillType key = accumulatorKeys[i];
            if (!_activeContinuousSkills.Contains(key))
            {
                _continuousAccumulators[key] = 0f;
            }
        }

        // Log các skill duy trì đang hoạt động mỗi giây 1 lần
        if (_activeContinuousSkills.Count > 0)
        {
            _statusLogTimer += Time.deltaTime;
            if (_statusLogTimer >= 1.0f)
            {
                string activeSkillsStr = string.Join(", ", _activeContinuousSkills);
                Debug.Log($"[SkillController] Active Skills: [{activeSkillsStr}] | Current Mana: {_manaState?.GetCurrentMana():F0}");
                _statusLogTimer = 0f;
            }
        }
        else
        {
            _statusLogTimer = 0f;
        }
        _activeContinuousSkills.Clear();
    }

    public BaseSkill GetSkillConfig(SkillType type)
    {
        for (int i = 0; i < _skills.Count; i++)
        {
            if (_skills[i] != null && _skills[i].skillType == type)
                return _skills[i];
        }
        return null;
    }

    public bool CanCast(SkillType type)
    {
        BaseSkill config = GetSkillConfig(type);
        if (config == null) return false;

        // 1. Kiểm tra Cooldown
        if (_cooldownTimers.ContainsKey(type) && _cooldownTimers[type] > 0) 
            return false;

        // 2. Kiểm tra Mana
        if (_manaState == null || !_manaState.HasEnoughMana(config.manaCost)) 
            return false;

        return true;
    }

    public void CastSkill(SkillType type)
    {
        BaseSkill config = GetSkillConfig(type);
        if (config == null) return;

        Debug.Log($"[SkillController] Cast Skill: {config.skillName} ({type}) | Mana Cost: {config.manaCost} | IsContinuous: {config.isContinuous}");

        // Trừ mana tick đầu tiên ngay lập tức
        _manaConsumer?.ConsumeMana(config.manaCost);

        if (!config.isContinuous)
        {
            _cooldownTimers[type] = config.cooldown;
        }
        else
        {
            _continuousAccumulators[type] = 0f;
        }
    }

    public bool ConsumeContinuousMana(SkillType type, float deltaTime)
    {
        BaseSkill config = GetSkillConfig(type);
        if (config == null || !config.isContinuous || _manaConsumer == null)
            return false;

        if (!_activeContinuousSkills.Contains(type))
        {
            _activeContinuousSkills.Add(type);
        }

        if (!_continuousAccumulators.ContainsKey(type))
        {
            _continuousAccumulators[type] = 0f;
        }

        _continuousAccumulators[type] += deltaTime;

        // Khi tích lũy đủ 1 giây hoạt động liên tục
        if (_continuousAccumulators[type] >= 1.0f)
        {
            if (_manaState != null && _manaState.HasEnoughMana(config.manaCost))
            {
                _manaConsumer?.ConsumeMana(config.manaCost);
                _continuousAccumulators[type] -= 1.0f;
            }
            else
            {
                return false; // Không đủ mana duy trì
            }
        }
        return true;
    }
}
