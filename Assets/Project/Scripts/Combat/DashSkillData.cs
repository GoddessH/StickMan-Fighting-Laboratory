using UnityEngine;

[CreateAssetMenu(fileName = "NewDashSkill", menuName = "Project/Skills/Dash Skill")]
public class DashSkillData : BaseSkill
{
    [Header("Dash Configuration")]
    public float dashDistance = 3f; // Khoảng cách lướt mặc định
}
