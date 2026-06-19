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
        else if (context.Config.ManaConfig.enableManaCharging && context.Mana != null)
        {
            float manaPercent = context.Mana.GetManaPercent();
            if (manaPercent <= context.Config.ManaConfig.chargeThresholdPercent &&
                manaPercent < context.Config.ManaConfig.chargeMaxPercent &&
                !context.Mana.IsFullMana())
            {
                context.ScheduleTransition(BotBrain.AIState.Charge);
            }
        }
    }
}
