using UnityEngine;

public class BotChargeState : BotState
{
    public BotChargeState(IBotContext context) : base(context) {}

    public override void Enter()
    {
        context.Executor.SetMovement(Vector2.zero);
        context.Executor.StartCharge();
    }

    public override void Update()
    {
        context.Executor.SetMovement(Vector2.zero);

        if (context.Mana != null)
        {
            float currentMana = context.Mana.GetCurrentMana();
            float maxMana = context.Mana.GetMaxMana();
            float currentManaPercent = maxMana > 0 ? (currentMana / maxMana) * 100f : 100f;

            // Nếu mana đã đầy hoặc đạt ngưỡng tối đa cấu hình, dừng sạc và tiếp cận mục tiêu
            if (context.Mana.IsFullMana() || currentManaPercent >= context.Config.ManaConfig.chargeMaxPercent)
            {
                context.ScheduleTransition(BotBrain.AIState.Approach);
            }
        }
        else
        {
            // Fallback nếu không tìm thấy bộ kiểm tra Mana
            context.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }

    public override void Exit()
    {
        context.Executor.StopCharge();
    }
}
