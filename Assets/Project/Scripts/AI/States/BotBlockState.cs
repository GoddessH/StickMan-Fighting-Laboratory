using UnityEngine;

public class BotBlockState : BotState
{
    private float _blockTimer;

    public override int Priority => 2;

    public BotBlockState(IBotContext context) : base(context) {}

    public override bool CanInterrupt(BotBrain.AIState incomingState)
    {
        if (_blockTimer <= 0) return true;
        // Đỡ đòn chỉ có thể bị ngắt bởi việc Rút lui (Retreat) khẩn cấp
        return incomingState == BotBrain.AIState.Retreat;
    }

    public override void Enter()
    {
        context.Executor.SetMovement(Vector2.zero);
        _blockTimer = context.Config.Timing.blockDuration;
        context.Executor.StartBlock();
    }

    public override void Update()
    {
        context.Executor.SetMovement(Vector2.zero);
        _blockTimer -= Time.deltaTime;

        bool threatStillDetected = context.Sensor.ThreatDetected;
        if (_blockTimer <= 0 || !threatStillDetected)
        {
            context.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }

    public override void Exit()
    {
        context.Executor.StopBlock();
    }
}
