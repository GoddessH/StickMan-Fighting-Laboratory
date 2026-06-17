using UnityEngine;

public class BotApproachState : BotState
{
    public BotApproachState(BotBrain brain, IBotSensor sensor, IBotExecutor executor) : base(brain, sensor, executor) {}

    public override void Update()
    {
        // Nếu đang chờ chuyển trạng thái (trong thời gian delay phản xạ), dừng di chuyển hoàn toàn
        if (brain.IsTransitionPending)
        {
            executor.SetMovement(Vector2.zero);
            return;
        }

        // Tính vector di chuyển: x theo hướng target, y theo độ cao tương đối
        float xMove = sensor.DirectionToTarget.x;
        float yMove = 0f;

        // Nếu đã ở trong tầm đánh, dừng di chuyển ngang để tránh đè/dính vào Player
        if (sensor.Distance <= config.attackRange)
        {
            xMove = 0f;
        }

        // Tự động điều chỉnh độ nhạy bay dựa trên AirborneScore của đối thủ
        float airborneScore = 0f;
        if (config.enablePatternTracking && brain.PatternTracker != null)
        {
            airborneScore = brain.PatternTracker.AirborneScore;
        }

        float dynamicFlyThreshold = config.flyThreshold;
        if (airborneScore > 0.3f)
        {
            // Độ nhạy tăng (ngưỡng giảm) khi đối thủ bay nhảy nhiều
            dynamicFlyThreshold = config.flyThreshold * (1f - airborneScore * 0.5f);
        }

        // Fly lên nếu Player cao hơn Bot quá ngưỡng dynamicFlyThreshold
        if (sensor.DistanceY > dynamicFlyThreshold && Mathf.Abs(sensor.DistanceY) <= config.maxVerticalChase)
        {
            yMove = 1f;
        }
        // Hạ xuống nếu Bot cao hơn Player
        else if (sensor.DistanceY < -dynamicFlyThreshold)
        {
            yMove = -1f;
        }

        executor.SetMovement(new Vector2(xMove, yMove));

        // Cần đảm bảo mục tiêu nằm trong tầm đánh cả chiều ngang và chiều dọc (thẳng hàng) trước khi đánh
        float diffX = Mathf.Abs(sensor.Target.position.x - brain.transform.position.x);
        float diffY = Mathf.Abs(sensor.DistanceY);
        bool inAttackRange = diffX <= config.attackRange && diffY <= dynamicFlyThreshold;

        // Vào attackRange + cooldown xong → chọn action
        if (inAttackRange && brain.AttackTimer <= 0)
        {
            BotBrain.AIState action = brain.PickAttackAction();
            brain.ScheduleTransition(action);
        }
        else if (sensor.Distance > config.detectRange)
        {
            brain.ScheduleTransition(BotBrain.AIState.Idle);
        }
    }
}
