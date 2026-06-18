using UnityEngine;

public class BotRetreatState : BotState
{
    private float _retreatTimer;

    public override int Priority => 2;

    public BotRetreatState(IBotContext context) : base(context) {}

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
            context.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }
}
