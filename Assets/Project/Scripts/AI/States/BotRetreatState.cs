using UnityEngine;

public class BotRetreatState : BotState
{
    private float _retreatTimer;

    public BotRetreatState(BotBrain brain, BotSensor sensor, BotExecutor executor) : base(brain, sensor, executor) {}

    public override void Enter()
    {
        _retreatTimer = config.retreatDuration;
        executor.StopBlock();
    }

    public override void Update()
    {
        _retreatTimer -= Time.deltaTime;
        executor.SetMovement(new Vector2(-sensor.DirectionToTarget.x, 0f));

        if (_retreatTimer <= 0)
        {
            brain.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }
}
