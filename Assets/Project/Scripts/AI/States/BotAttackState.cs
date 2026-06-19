using UnityEngine;

public class BotAttackState : BotState
{
    private float _attackSequenceTimer;
    private float _nextComboHitTimer;

    public override int Priority => 1;

    public BotAttackState(IBotContext context) : base(context) {}

    public override bool CanInterrupt(BotBrain.AIState incomingState)
    {
        // Khi đang thực hiện chuỗi tấn công combo, không cho phép bất kỳ trạng thái nào ngắt (tránh việc Bot tự hủy đòn đánh để đỡ/lùi)
        return _attackSequenceTimer <= 0;
    }

    public override void Enter()
    {
        context.Executor.SetMovement(Vector2.zero);

        // Chọn số đòn combo ngẫu nhiên từ 1 đến maxComboHits
        int comboHits = Random.Range(1, context.Config.Timing.maxComboHits + 1);
        _attackSequenceTimer = comboHits * context.Config.Timing.singleAttackDuration;
        _nextComboHitTimer = context.Config.Timing.singleAttackDuration;

        context.SetAttackCooldown();
        context.Executor.TriggerAttack(); // kích hoạt đòn đầu tiên
    }

    public override void Update()
    {
        context.Executor.SetMovement(Vector2.zero);

        // Duy trì kích hoạt combo bằng các đòn chém rời rạc đúng thời điểm thay vì spam liên tục giữ nút
        if (_attackSequenceTimer > 0)
        {
            _attackSequenceTimer -= Time.deltaTime;
            _nextComboHitTimer -= Time.deltaTime;
            if (_nextComboHitTimer <= 0 && _attackSequenceTimer > 0)
            {
                context.Executor.TriggerAttack();
                _nextComboHitTimer = context.Config.Timing.singleAttackDuration;
            }
        }
        else
        {
            // Chỉ lập lịch di chuyển tiếp cận khi chuỗi combo đã thực sự kết thúc
            context.ScheduleTransition(BotBrain.AIState.Approach);
        }
    }

    public override void Exit()
    {
        CancelCombo();
    }

    private void CancelCombo()
    {
        _attackSequenceTimer = 0f;
        _nextComboHitTimer = 0f;
    }
}
