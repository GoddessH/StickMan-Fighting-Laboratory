using UnityEngine;

public class BotChargeState : BotState
{
    private float _chargeDurationTimer;

    public BotChargeState(IBotContext context) : base(context) {}

    public override void Enter()
    {
        context.Executor.SetMovement(Vector2.zero);
        context.Executor.StartCharge();
        // Lấy thời gian sạc tối đa từ cấu hình (mặc định 1.5 giây nếu cấu hình rỗng)
        _chargeDurationTimer = context.Config != null ? context.Config.ManaConfig.maxChargeDuration : 1.5f;
    }

    public override void Update()
    {
        context.Executor.SetMovement(Vector2.zero);
        context.Executor.StartCharge(); // Gọi liên tục mỗi frame để duy trì sạc và tự động kích hoạt lại nếu bị gián đoạn (như trúng đòn Hurt)

        // Giảm thời gian sạc tối đa
        _chargeDurationTimer -= Time.deltaTime;
        if (_chargeDurationTimer <= 0)
        {
            context.ScheduleTransition(BotBrain.AIState.Approach);
            return;
        }

        if (context.Mana != null)
        {
            // Sử dụng extension GetManaPercent() giúp rút ngắn code và tránh lặp phép tính
            if (context.Mana.IsFullMana() || context.Mana.GetManaPercent() >= context.Config.ManaConfig.chargeMaxPercent)
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
