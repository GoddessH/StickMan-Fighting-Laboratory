using System;
using UnityEngine;

public interface IBotContext
{
    BotDifficultyConfig Config { get; }
    IBotSensor Sensor { get; }
    IBotExecutor Executor { get; }
    PlayerPatternTracker PatternTracker { get; }
    float AttackTimer { get; }
    bool IsTransitionPending { get; }
    Transform transform { get; }

    event Action<BotBrain.AIState, BotBrain.AIState> OnStateChanged;
    event Action OnTargetLost;
    event Action<Transform> OnTargetAcquired;
    event Action<float> OnDamageTaken;

    void ScheduleTransition(BotBrain.AIState nextState);
    void SetAttackCooldown();
    BotBrain.AIState PickAttackAction();
    BotBrain.AIState PickThreatReaction();
}
