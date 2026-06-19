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
            if (context.Config.ManaConfig.enableManaCharging && context.Mana != null)
            {
                float currentMana = context.Mana.GetCurrentMana();
                float maxMana = context.Mana.GetMaxMana();
                float currentManaPercent = maxMana > 0 ? (currentMana / maxMana) * 100f : 100f;

                if (currentManaPercent <= context.Config.ManaConfig.chargeThresholdPercent && 
                    context.Sensor.Distance > context.Config.Detection.threatRange)
                {
                    context.ScheduleTransition(BotBrain.AIState.Charge);
                    return;
                }
            }
            context.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }
}
