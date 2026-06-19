using UnityEngine;

public class BotApproachState : BotState
{
    private readonly PlayerPatternTracker _patternTracker;

    public BotApproachState(IBotContext context, PlayerPatternTracker patternTracker) : base(context)
    {
        _patternTracker = patternTracker;
    }

    public override void Enter()
    {
        context.Executor.SetMovement(Vector2.zero); // Reset về trung tính
    }

    public override void Update()
    {
        float diffX = context.Sensor.DistanceX;
        float diffY = Mathf.Abs(context.Sensor.DistanceY);

        // 1. Tự động kiểm tra Mana để Sạc nếu an toàn
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

        // Tự động điều chỉnh độ nhạy bay dựa trên AirborneScore của đối thủ
        float airborneScore = 0f;
        if (context.Config.Pattern.enablePatternTracking && _patternTracker != null)
        {
            airborneScore = _patternTracker.AirborneScore;
        }

        float dynamicFlyThreshold = context.Config.Movement.flyThreshold;
        if (airborneScore > 0.3f)
        {
            // Độ nhạy tăng (ngưỡng giảm) khi đối thủ bay nhảy nhiều
            dynamicFlyThreshold = context.Config.Movement.flyThreshold * (1f - airborneScore * 0.5f);
        }

        // 2. Tự động dùng Flash để áp sát khi ở khoảng cách xa phương ngang và độ cao tương đối gần (nới lỏng sai lệch dọc)
        if (context.SkillController != null && context.SkillController.CanCast(SkillType.Flash))
        {
            if (diffX > context.Config.Detection.threatRange && diffX <= 6f && diffY <= 1.2f)
            {
                context.Executor.TriggerFlash();
            }
        }

        // Tính vector di chuyển: x theo hướng target, y theo độ cao tương đối
        float xMove = context.Sensor.DirectionToTarget.x;
        float yMove = 0f;

        // Nếu đã ở trong tầm đánh, dừng di chuyển ngang để tránh đè/dính vào Player
        if (context.Sensor.Distance <= context.Config.Detection.attackRange)
        {
            xMove = 0f;
        }

        // Fly lên nếu Player cao hơn Bot quá ngưỡng dynamicFlyThreshold
        if (context.Sensor.DistanceY > dynamicFlyThreshold && Mathf.Abs(context.Sensor.DistanceY) <= context.Config.Movement.maxVerticalChase)
        {
            yMove = 1f;
        }
        // Hạ xuống nếu Bot cao hơn Player
        else if (context.Sensor.DistanceY < -dynamicFlyThreshold)
        {
            yMove = -1f;
        }

        context.Executor.SetMovement(new Vector2(xMove, yMove));

        // Cần đảm bảo mục tiêu nằm trong tầm đánh cả chiều ngang và chiều dọc (nới lỏng sai lệch dọc để dễ ra đòn)
        float verticalAttackTolerance = Mathf.Max(dynamicFlyThreshold, 1.0f);
        bool inAttackRange = diffX <= context.Config.Detection.attackRange && diffY <= verticalAttackTolerance;

        // Vào attackRange + cooldown xong → chuyển sang Attack
        if (inAttackRange && context.AttackTimer <= 0)
        {
            context.ScheduleTransition(BotBrain.AIState.Attack);
        }
        else if (context.Sensor.Distance > context.Config.Detection.detectRange)
        {
            context.ScheduleTransition(BotBrain.AIState.Idle);
        }
    }
}
