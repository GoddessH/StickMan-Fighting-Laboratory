using System;
using UnityEngine;

public interface IBotContext
{
    BotDifficultyConfig Config { get; }
    IBotSensor Sensor { get; }
    IBotExecutor Executor { get; }
    float AttackTimer { get; }
    bool IsTransitionPending { get; }
    IManaChecker Mana { get; }
    SkillController SkillController { get; }

    void ScheduleTransition(BotBrain.AIState nextState);
    void SetAttackCooldown();
}
