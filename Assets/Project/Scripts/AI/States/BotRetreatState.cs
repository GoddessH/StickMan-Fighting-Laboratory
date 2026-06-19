using UnityEngine;

public class BotRetreatState : BotState
{
    private float _retreatTimer;

    public override int Priority => 2;

    public BotRetreatState(IBotContext context) : base(context) {}

    public override bool CanInterrupt(BotBrain.AIState incomingState)
    {
        if (_retreatTimer <= 0) return true;
        // Rút lui chỉ có thể bị ngắt bởi Đỡ đòn (Block) khẩn cấp
        return incomingState == BotBrain.AIState.Block;
    }

    public override void Enter()
    {
        _retreatTimer = context.Config.Timing.retreatDuration;
        context.Executor.StopBlock();
    }

    public override void Update()
    {
        _retreatTimer -= Time.deltaTime;
        context.Executor.SetMovement(new Vector2(-context.Sensor.DirectionToTarget.x, 0f));

        if (_retreatTimer <= 0)
        {
            context.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }
}
