using UnityEngine;

public class BotBlockState : BotState
{
    private float _blockTimer;

    public BotBlockState(BotBrain brain, BotSensor sensor, BotExecutor executor) : base(brain, sensor, executor) {}

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
            brain.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }

    public override void Exit()
    {
        executor.StopBlock();
    }
}
