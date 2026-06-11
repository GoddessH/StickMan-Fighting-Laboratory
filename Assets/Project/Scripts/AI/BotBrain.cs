using UnityEngine;

[RequireComponent(typeof(BotInput))]
public class BotBrain : MonoBehaviour
{
    // ---- Cấu hình trong Inspector ----
    [Header("Target")]
    [SerializeField] private Transform _target;

    [Header("Difficulty")]
    [SerializeField] private BotDifficultyConfig _config;

    [Header("Threat Detection")]
    [Tooltip("Tag của GameObject chứa AttackHitBox của Player")]
    [SerializeField] private string _playerHitBoxTag = "PlayerHitBox";

    // ---- AI State nội bộ ----
    private enum AIState { Idle, Approach, Attack, Block, Retreat }
    [Header("Debug Info")]
    [SerializeField] private AIState _currentAIState = AIState.Idle;

    // ---- Ref ----
    private BotInput _botInput;
    private RotationHandler _rotationHandler;

    // ---- Timers ----
    private float _attackTimer;
    private float _blockTimer;
    private float _retreatTimer;
    private float _reactionTimer;
    private float _attackSequenceTimer;
    private float _nextComboHitTimer;

    // ---- Pending transition (chờ reaction delay) ----
    private AIState? _pendingState;

    private void Awake()
    {
        _botInput = GetComponent<BotInput>();
        _rotationHandler = GetComponent<RotationHandler>();
    }

    private void Update()
    {
        // Tự động tìm Player nếu chưa được gán
        if (_target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                _target = player.transform;
            }
        }

        // Đồng bộ target cho RotationHandler để Bot luôn xoay hướng về phía Player
        if (_target != null && _rotationHandler != null)
        {
            _rotationHandler.SetTarget(_target);
        }

        // Đảm bảo BotInput dọn dẹp trạng thái input frame trước
        if (_botInput != null && _botInput.BotAttack != null)
        {
            _botInput.BotAttack.Clear();
        }

        if (_target == null || _config == null) return;

        _attackTimer -= Time.deltaTime;
        _blockTimer -= Time.deltaTime;
        _retreatTimer -= Time.deltaTime;
        _reactionTimer -= Time.deltaTime;

        // Thực hiện pending transition sau khi reaction delay xong
        if (_pendingState.HasValue && _reactionTimer <= 0)
        {
            ExecuteTransition(_pendingState.Value);
            _pendingState = null;
        }

        Vector2 toTarget = (Vector2)(_target.position - transform.position);
        float dist = toTarget.magnitude;
        float distY = _target.position.y - transform.position.y;

        // Phát hiện threat mỗi frame (không phụ thuộc AI State)
        bool threatDetected = DetectPlayerHitBox();

        TickAI(dist, distY, toTarget.normalized, threatDetected);
    }

    // -------- Threat Detection --------
    // Dùng Physics2D.OverlapCircleAll — KHÔNG đọc trực tiếp Player input
    private bool DetectPlayerHitBox()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            _config.threatRange
        );

        foreach (var hit in hits)
        {
            if (hit != null && hit.CompareTag(_playerHitBoxTag))
            {
                return true;
            }
        }
        return false;
    }

    // -------- FSM --------
    private void TickAI(float dist, float distY, Vector2 dirToTarget, bool threatDetected)
    {
        // Threat override: Nếu phát hiện nguy hiểm và không đang Block
        // → dùng Weighted Random chọn Block hay Retreat (cho phép phản xạ khi đang Retreat)
        if (threatDetected &&
            _currentAIState != AIState.Block &&
            !_pendingState.HasValue)
        {
            AIState reaction = PickThreatReaction();
            Debug.Log($"[BotBrain] Threat detected! → {reaction}");
            ScheduleTransition(reaction);
            return;
        }

        switch (_currentAIState)
        {
            case AIState.Idle:
                HandleIdle(dist);
                break;
            case AIState.Approach:
                HandleApproach(dist, distY, dirToTarget);
                break;
            case AIState.Attack:
                HandleAttack();
                break;
            case AIState.Block:
                HandleBlock();
                break;
            case AIState.Retreat:
                HandleRetreat(dirToTarget);
                break;
        }
    }

    // -------- State Handlers --------

    private void HandleIdle(float dist)
    {
        _botInput.BotMovement.SetDirection(Vector2.zero);
        if (dist <= _config.detectRange)
        {
            ScheduleTransition(AIState.Approach);
        }
    }

    private void HandleApproach(float dist, float distY, Vector2 dirToTarget)
    {
        // Nếu đang chờ chuyển trạng thái (trong thời gian delay phản xạ), dừng di chuyển hoàn toàn
        if (_pendingState.HasValue)
        {
            _botInput.BotMovement.SetDirection(Vector2.zero);
            return;
        }

        // Tính vector di chuyển: x theo hướng target, y theo độ cao tương đối
        float xMove = dirToTarget.x;
        float yMove = 0f;

        // Nếu đã ở trong tầm đánh, dừng di chuyển ngang để tránh đè/dính vào Player
        if (dist <= _config.attackRange)
        {
            xMove = 0f;
        }

        // Fly lên nếu Player cao hơn Bot quá ngưỡng flyThreshold
        if (distY > _config.flyThreshold && Mathf.Abs(distY) <= _config.maxVerticalChase)
        {
            yMove = 1f;  // y > 0 → FlyHandler bật isFly = true
        }
        // Hạ xuống nếu Bot cao hơn Player
        else if (distY < -_config.flyThreshold)
        {
            yMove = -1f; // y < 0 → di chuyển xuống (FallHandler tự xử lý rơi)
        }

        _botInput.BotMovement.SetDirection(new Vector2(xMove, yMove));

        // Cần đảm bảo mục tiêu nằm trong tầm đánh cả chiều ngang và chiều dọc (thẳng hàng) trước khi đánh
        float diffX = Mathf.Abs(_target.position.x - transform.position.x);
        float diffY = Mathf.Abs(distY);
        bool inAttackRange = diffX <= _config.attackRange && diffY <= _config.flyThreshold;

        // Vào attackRange + cooldown xong → chọn action
        if (inAttackRange && _attackTimer <= 0)
        {
            AIState action = PickAttackAction();
            ScheduleTransition(action);
        }
        else if (dist > _config.detectRange)
        {
            ScheduleTransition(AIState.Idle);
        }
    }

    private void HandleAttack()
    {
        _botInput.BotMovement.SetDirection(Vector2.zero);

        // Duy trì kích hoạt combo bằng các đòn chém rời rạc đúng thời điểm thay vì spam liên tục giữ nút
        if (_attackSequenceTimer > 0)
        {
            _attackSequenceTimer -= Time.deltaTime;
            _nextComboHitTimer -= Time.deltaTime;
            if (_nextComboHitTimer <= 0 && _attackSequenceTimer > 0)
            {
                _botInput.BotAttack.TriggerAttack();
                _nextComboHitTimer = _config.singleAttackDuration;
            }
        }
        else
        {
            // Chỉ lập lịch di chuyển tiếp cận khi chuỗi combo đã thực sự kết thúc
            ScheduleTransition(AIState.Approach);
        }
    }

    private void HandleBlock()
    {
        _botInput.BotMovement.SetDirection(Vector2.zero);
        bool threatStillDetected = DetectPlayerHitBox();
        if (_blockTimer <= 0 || !threatStillDetected)
        {
            _botInput.BotBlock.StopBlock();
            ScheduleTransition(AIState.Approach);
        }
    }

    private void HandleRetreat(Vector2 dirToTarget)
    {
        // Di chuyển ngược chiều Player (lùi ra)
        _botInput.BotMovement.SetDirection(new Vector2(-dirToTarget.x, 0));
        if (_retreatTimer <= 0)
        {
            ScheduleTransition(AIState.Approach);
        }
    }

    // -------- Weighted Random --------

    // Khi vào attackRange: chọn Attack, hoặc Idle (sai lầm/đứng yên)
    private AIState PickAttackAction()
    {
        float total = _config.attackWeight + _config.idleWeight;
        float roll = Random.Range(0f, total);
        return roll < _config.attackWeight ? AIState.Attack : AIState.Idle;
    }

    // Khi phát hiện threat: chọn Block, Retreat, hoặc Idle (không phản ứng)
    private AIState PickThreatReaction()
    {
        float total = _config.blockWeight + _config.retreatWeight + _config.idleWeight;
        float roll = Random.Range(0f, total);

        if (roll < _config.blockWeight) return AIState.Block;
        roll -= _config.blockWeight;

        if (roll < _config.retreatWeight) return AIState.Retreat;

        return AIState.Idle;
    }

    // -------- Reaction Delay --------

    private void ScheduleTransition(AIState nextState)
    {
        if (_pendingState == nextState) return;
        _pendingState = nextState;

        // Phản xạ tự vệ (Block/Retreat) nhanh hơn ~4x so với di chuyển chủ động
        if (nextState == AIState.Block || nextState == AIState.Retreat)
            _reactionTimer = _config.reactionDelay * 0.25f;
        else
            _reactionTimer = _config.reactionDelay;
    }

    private void ExecuteTransition(AIState nextState)
    {
        _currentAIState = nextState;
        Debug.Log($"[BotBrain] → {nextState}");

        switch (nextState)
        {
            case AIState.Attack:
                _botInput.BotMovement.SetDirection(Vector2.zero);
                // Chọn số đòn combo ngẫu nhiên từ 1 đến maxComboHits
                int comboHits = Random.Range(1, _config.maxComboHits + 1);
                _attackSequenceTimer = comboHits * _config.singleAttackDuration;
                _nextComboHitTimer = _config.singleAttackDuration;
                _attackTimer = _config.attackCooldown;
                _botInput.BotAttack.TriggerAttack(); // kích hoạt đòn đầu tiên
                break;

            case AIState.Block:
                _blockTimer = _config.blockDuration;
                _botInput.BotBlock.StartBlock();
                break;
            case AIState.Retreat:
                _retreatTimer = _config.retreatDuration;
                _botInput.BotBlock.StopBlock();
                break;
            case AIState.Approach:
            case AIState.Idle:
                // Đảm bảo block được tắt khi rời khỏi Block state
                _botInput.BotBlock.StopBlock();
                break;
        }
    }

    // -------- Public API --------

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void SetConfig(BotDifficultyConfig config)
    {
        _config = config;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_config == null) return;

        // Green: Detect range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _config.detectRange);

        // Yellow: Threat range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _config.threatRange);

        // Red: Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _config.attackRange);
    }
#endif
}
