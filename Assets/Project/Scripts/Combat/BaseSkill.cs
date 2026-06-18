using UnityEngine;

[CreateAssetMenu(fileName = "NewBaseSkill", menuName = "Project/Skills/Base Skill")]
public class BaseSkill : ScriptableObject
{
    [Header("Skill Identity")]
    public SkillType skillType;
    public string skillName;

    [Header("Cost & Cooldown")]
    public float manaCost;
    public bool isContinuous; // True nếu trừ mana mỗi giây, False nếu trừ tức thời
    public float cooldown;

    [Header("Animation")]
    public string animationBoolOrTriggerName;

    public virtual Skill CreateInstance()
    {
        return new MockSkill();
    }
}

public enum SkillType
{
    Block,
    Flash,
    Skill1,
    Skill2,
    Skill3,
    Skill4
}
