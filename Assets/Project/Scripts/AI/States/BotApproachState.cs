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
        // Nếu đang chờ chuyển trạng thái (trong thời gian delay phản xạ), dừng di chuyển hoàn toàn
        if (context.IsTransitionPending)
        {
            context.Executor.SetMovement(Vector2.zero);
            return;
        }

        // Tính vector di chuyển: x theo hướng target, y theo độ cao tương đối
        float xMove = context.Sensor.DirectionToTarget.x;
        float yMove = 0f;

        // Nếu đã ở trong tầm đánh, dừng di chuyển ngang để tránh đè/dính vào Player
        if (context.Sensor.Distance <= context.Config.Detection.attackRange)
        {
            xMove = 0f;
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

        // Cần đảm bảo mục tiêu nằm trong tầm đánh cả chiều ngang và chiều dọc (thẳng hàng) trước khi đánh
        float diffX = context.Sensor.DistanceX;
        float diffY = Mathf.Abs(context.Sensor.DistanceY);
        bool inAttackRange = diffX <= context.Config.Detection.attackRange && diffY <= dynamicFlyThreshold;

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
