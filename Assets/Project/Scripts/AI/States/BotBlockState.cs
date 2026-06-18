using UnityEngine;

public class BotBlockState : BotState
{
    private float _blockTimer;

    public override int Priority => 2;

    public BotBlockState(IBotContext context) : base(context) {}

    public override void Enter()
    {
        executor.SetMovement(Vector2.zero);
        _blockTimer = config.blockDuration;
        executor.StartBlock();
    }

    public override void Update()
    {
        executor.SetMovement(Vector2.zero);
        _blockTimer -= Time.deltaTime;

        bool threatStillDetected = sensor.ThreatDetected;
        if (_blockTimer <= 0 || !threatStillDetected)
        {
            context.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }

    public override void Exit()
    {
        executor.StopBlock();
    }
}
