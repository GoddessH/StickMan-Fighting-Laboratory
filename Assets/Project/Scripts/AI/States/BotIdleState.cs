using UnityEngine;

public class BotIdleState : BotState
{
    public BotIdleState(IBotContext context) : base(context) {}

    public override void Enter()
    {
        executor.SetMovement(Vector2.zero);
    }

    public override void Update()
    {
        if (sensor.Distance <= config.detectRange)
        {
            context.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }
}
