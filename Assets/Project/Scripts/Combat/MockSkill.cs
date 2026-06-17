using UnityEngine;

public class MockSkill : Skill
{
    protected override void ExecuteLogic()
    {
        Debug.Log($"[MockSkill] Executing skill: {Config.skillName} on slot: {Config.skillType}");
        if (!Config.isContinuous)
        {
            OnEnd();
        }
    }
}
