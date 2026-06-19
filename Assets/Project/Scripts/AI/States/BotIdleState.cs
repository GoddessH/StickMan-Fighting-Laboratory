using UnityEngine;

public class BotIdleState : BotState
{
    public BotIdleState(IBotContext context) : base(context) {}

    public override void Enter()
    {
        context.Executor.SetMovement(Vector2.zero);
    }

    public override void Update()
    {
        if (context.Sensor.Distance <= context.Config.Detection.detectRange)
        {
            context.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }
}
