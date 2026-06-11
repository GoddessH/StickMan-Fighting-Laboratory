using UnityEngine;

public class BotAttackState : BotState
{
    private float _attackSequenceTimer;
    private float _nextComboHitTimer;

    public BotAttackState(BotBrain brain, BotSensor sensor, BotExecutor executor) : base(brain, sensor, executor) {}

    public override void Enter()
    {
        executor.SetMovement(Vector2.zero);

        // Chọn số đòn combo ngẫu nhiên từ 1 đến maxComboHits
        int comboHits = Random.Range(1, config.maxComboHits + 1);
        _attackSequenceTimer = comboHits * config.singleAttackDuration;
        _nextComboHitTimer = config.singleAttackDuration;

        brain.SetAttackCooldown();
        executor.TriggerAttack(); // kích hoạt đòn đầu tiên
    }

    public override void Update()
    {
        executor.SetMovement(Vector2.zero);

        // Duy trì kích hoạt combo bằng các đòn chém rời rạc đúng thời điểm thay vì spam liên tục giữ nút
        if (_attackSequenceTimer > 0)
        {
            _attackSequenceTimer -= Time.deltaTime;
            _nextComboHitTimer -= Time.deltaTime;
            if (_nextComboHitTimer <= 0 && _attackSequenceTimer > 0)
            {
                executor.TriggerAttack();
                _nextComboHitTimer = config.singleAttackDuration;
            }
        }
        else
        {
            // Chỉ lập lịch di chuyển tiếp cận khi chuỗi combo đã thực sự kết thúc
            brain.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }
}
