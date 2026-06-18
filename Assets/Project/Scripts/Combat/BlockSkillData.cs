using UnityEngine;

[CreateAssetMenu(fileName = "NewBlockSkill", menuName = "Project/Skills/Block Skill")]
public class BlockSkillData : BaseSkill
{
    public override Skill CreateInstance()
    {
        return new BlockSkill();
    }
}
