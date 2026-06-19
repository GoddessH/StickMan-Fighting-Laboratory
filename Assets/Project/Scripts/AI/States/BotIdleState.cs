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
            float currentMana = context.Mana.GetCurrentMana();
            float maxMana = context.Mana.GetMaxMana();
            float currentManaPercent = maxMana > 0 ? (currentMana / maxMana) * 100f : 100f;

            if (currentManaPercent <= context.Config.ManaConfig.chargeThresholdPercent)
            {
                context.ScheduleTransition(BotBrain.AIState.Charge);
            }
        }
    }
}
