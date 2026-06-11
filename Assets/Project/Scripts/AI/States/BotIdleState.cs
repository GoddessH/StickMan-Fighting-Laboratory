using UnityEngine;

public class BotIdleState : BotState
{
    public BotIdleState(BotBrain brain, BotSensor sensor, BotExecutor executor) : base(brain, sensor, executor) {}

    public override void Enter()
    {
        executor.SetMovement(Vector2.zero);
    }

    public override void Update()
    {
        if (sensor.Distance <= config.detectRange)
        {
            brain.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }
}
