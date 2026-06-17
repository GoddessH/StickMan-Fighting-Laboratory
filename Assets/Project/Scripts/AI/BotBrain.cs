using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BotInput))]
public class BotBrain : MonoBehaviour
{
    // ---- Cấu hình trong Inspector (Giữ nguyên để bảo toàn dữ liệu trên Prefabs) ----
    [Header("Target")]
    [SerializeField] private Transform _target;

    [Header("Difficulty")]
    [SerializeField] private BotDifficultyConfig _config;

    [Header("Threat Detection")]
    [Tooltip("LayerMask chứa AttackHitBox của Player")]
    [SerializeField] private LayerMask _playerHitBoxMask;

    // ---- AI State nội bộ ----
    public enum AIState { Idle, Approach, Attack, Block, Retreat }
    [Header("Debug Info")]
    [SerializeField] private AIState _currentAIState = AIState.Idle;

    // ---- Events ----
    public event System.Action<AIState, AIState> OnStateChanged;

    // ---- Refs ----
    private BotSensor _sensor;
    private BotExecutor _executor;
    private PlayerPatternTracker _patternTracker;

    // ---- States Map ----
    private Dictionary<AIState, BotState> _statesMap;
    private BotState _currentState;

    // ---- Timers ----
    private float _attackTimer;
    private float _reactionTimer;

    // ---- Pending transition (chờ reaction delay) ----
    private AIState? _pendingState;

    // ---- Public Properties ----
    public BotDifficultyConfig Config => _config;
    public float AttackTimer => _attackTimer;
    public bool IsTransitionPending => _pendingState.HasValue;
    public PlayerPatternTracker PatternTracker => _patternTracker;

    private void Awake()
    {
        // Khởi tạo các component bổ trợ, tự động gắn nếu chưa có
        _sensor = GetComponent<BotSensor>();
        if (_sensor == null)
        {
            _sensor = gameObject.AddComponent<BotSensor>();
        }
        _sensor.Init(_config, _playerHitBoxMask);

        _executor = GetComponent<BotExecutor>();
        if (_executor == null)
        {
            _executor = gameObject.AddComponent<BotExecutor>();
        }
        _executor.Init();

        _patternTracker = GetComponent<PlayerPatternTracker>();
        if (_patternTracker == null)
        {
            _patternTracker = gameObject.AddComponent<PlayerPatternTracker>();
        }
        _patternTracker.Init(_config, _target);

        // Khởi tạo các State chuyên biệt
        _statesMap = new Dictionary<AIState, BotState>
        {
            { AIState.Idle, new BotIdleState(this, _sensor, _executor) },
            { AIState.Approach, new BotApproachState(this, _sensor, _executor) },
            { AIState.Attack, new BotAttackState(this, _sensor, _executor) },
            { AIState.Block, new BotBlockState(this, _sensor, _executor) },
            { AIState.Retreat, new BotRetreatState(this, _sensor, _executor) }
        };
    }

    private void Start()
    {
        if (_target != null)
        {
            _sensor.SetTarget(_target);
            if (_patternTracker != null)
            {
                _patternTracker.SetTarget(_target);
            }
        }

        // Vào trạng thái mặc định ban đầu
        _currentAIState = AIState.Idle;
        _currentState = _statesMap[_currentAIState];
        _currentState.Enter();
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
                _sensor.SetTarget(_target);
                if (_patternTracker != null)
                {
                    _patternTracker.SetTarget(_target);
                }
            }
        }

        // Đảm bảo dọn dẹp trạng thái input frame trước
        _executor.ClearAttackInput();

        if (_target == null || _config == null) return;

        // Cập nhật cảm biến
        _sensor.UpdateSensor();

        // Giảm thời gian đếm ngược
        _attackTimer -= Time.deltaTime;
        _reactionTimer -= Time.deltaTime;

        // Thực hiện pending transition sau khi reaction delay xong
        if (_pendingState.HasValue && _reactionTimer <= 0)
        {
            ExecuteTransition(_pendingState.Value);
            _pendingState = null;
        }

        // Threat override: Nếu phát hiện nguy hiểm và không đang Block
        // → dùng Weighted Random chọn Block hay Retreat (cho phép phản xạ khi đang Retreat)
        if (_sensor.ThreatDetected &&
            _currentAIState != AIState.Block &&
            !_pendingState.HasValue)
        {
            AIState reaction = PickThreatReaction();
            ScheduleTransition(reaction);
            return;
        }

        // Cập nhật logic của trạng thái hiện tại
        _currentState.Update();
    }

    // -------- Weighted Random --------

    // Khi vào attackRange: chọn Attack, hoặc Idle (sai lầm/đứng yên)
    public AIState PickAttackAction()
    {
        float attackWeight = _config.attackWeight;
        float idleWeight = _config.idleWeight;

        if (_config.enablePatternTracking && _patternTracker != null)
        {
            // Nếu người chơi thủ nhiều (DefensivenessScore cao), bot tăng tấn công để tạo áp lực
            float defScale = _patternTracker.DefensivenessScore * _config.patternAdaptationStrength;
            attackWeight += _config.attackWeight * defScale;

            if (defScale > 0.3f)
            {
                Debug.Log($"[BotBrain] Thích ứng: Người chơi thủ nhiều (Defensiveness: {_patternTracker.DefensivenessScore:F2}), tăng attackWeight -> {attackWeight:F1}");
            }
        }

        float total = attackWeight + idleWeight;
        float roll = Random.Range(0f, total);
        return roll < attackWeight ? AIState.Attack : AIState.Idle;
    }

    // Khi phát hiện threat: chọn Block, Retreat, hoặc Idle (không phản ứng)
    public AIState PickThreatReaction()
    {
        float blockWeight = _config.blockWeight;
        float retreatWeight = _config.retreatWeight;
        float idleWeight = _config.idleWeight;

        if (_config.enablePatternTracking && _patternTracker != null)
        {
            // Nếu người chơi tấn công nhiều (AggressionScore cao), bot tăng block/retreat để phản xạ
            float aggressionScale = _patternTracker.AggressionScore * _config.patternAdaptationStrength;
            blockWeight += _config.blockWeight * aggressionScale;
            retreatWeight += _config.retreatWeight * aggressionScale;

            // Bot ít đứng im hơn khi người chơi quá hung hãn
            idleWeight = Mathf.Max(0f, idleWeight - idleWeight * aggressionScale * 0.5f);

            Debug.Log($"[BotBrain] Thích ứng: Người chơi tấn công nhiều (Aggression: {_patternTracker.AggressionScore:F2}), tăng blockWeight -> {blockWeight:F1}, retreatWeight -> {retreatWeight:F1}");
        }

        float total = blockWeight + retreatWeight + idleWeight;
        float roll = Random.Range(0f, total);

        if (roll < blockWeight) return AIState.Block;
        roll -= blockWeight;

        if (roll < retreatWeight) return AIState.Retreat;

        return AIState.Idle;
    }

    // -------- Reaction Delay & Transition --------

    public void ScheduleTransition(AIState nextState)
    {
        if (_pendingState == nextState) return;

        // Ngăn chặn trạng thái độ ưu tiên thấp hơn đè lên trạng thái độ ưu tiên cao hơn đang chờ xử lý
        if (_pendingState.HasValue && GetStatePriority(nextState) < GetStatePriority(_pendingState.Value))
        {
            return;
        }

        _pendingState = nextState;

        // Phản xạ tự vệ (Block/Retreat) nhanh hơn ~4x so với di chuyển chủ động
        if (nextState == AIState.Block || nextState == AIState.Retreat)
            _reactionTimer = _config.reactionDelay * 0.25f;
        else
            _reactionTimer = _config.reactionDelay;
    }

    private int GetStatePriority(AIState state)
    {
        switch (state)
        {
            case AIState.Block:
            case AIState.Retreat:
                return 2; // High priority (threat reactions)
            case AIState.Attack:
                return 1; // Medium priority (attacking)
            case AIState.Idle:
            case AIState.Approach:
            default:
                return 0; // Low priority (standard movement/idle)
        }
    }

    private void ExecuteTransition(AIState nextState)
    {
        AIState prevState = _currentAIState;
        _currentState.Exit();
        _currentAIState = nextState;
        _currentState = _statesMap[nextState];
        _currentState.Enter();
        OnStateChanged?.Invoke(prevState, nextState);
    }

    // -------- Public API --------

    public void SetAttackCooldown()
    {
        _attackTimer = _config.attackCooldown;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        if (_sensor != null)
        {
            _sensor.SetTarget(target);
        }
        if (_patternTracker != null)
        {
            _patternTracker.SetTarget(target);
        }
    }

    public void SetConfig(BotDifficultyConfig config)
    {
        _config = config;
        if (_sensor != null)
        {
            _sensor.Init(config, _playerHitBoxMask);
        }
        if (_patternTracker != null)
        {
            _patternTracker.Init(config, _target);
        }
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
