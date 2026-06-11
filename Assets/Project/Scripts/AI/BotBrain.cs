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
    [Tooltip("Tag của GameObject chứa AttackHitBox của Player")]
    [SerializeField] private string _playerHitBoxTag = "PlayerHitBox";

    // ---- AI State nội bộ ----
    public enum AIState { Idle, Approach, Attack, Block, Retreat }
    [Header("Debug Info")]
    [SerializeField] private AIState _currentAIState = AIState.Idle;

    // ---- Refs ----
    private BotSensor _sensor;
    private BotExecutor _executor;

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

    private void Awake()
    {
        // Khởi tạo các component bổ trợ, tự động gắn nếu chưa có
        _sensor = GetComponent<BotSensor>();
        if (_sensor == null)
        {
            _sensor = gameObject.AddComponent<BotSensor>();
        }
        _sensor.Init(_config, _playerHitBoxTag);

        _executor = GetComponent<BotExecutor>();
        if (_executor == null)
        {
            _executor = gameObject.AddComponent<BotExecutor>();
        }
        _executor.Init();

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
            Debug.Log($"[BotBrain] Threat detected! → {reaction}");
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
        float total = _config.attackWeight + _config.idleWeight;
        float roll = Random.Range(0f, total);
        return roll < _config.attackWeight ? AIState.Attack : AIState.Idle;
    }

    // Khi phát hiện threat: chọn Block, Retreat, hoặc Idle (không phản ứng)
    public AIState PickThreatReaction()
    {
        float total = _config.blockWeight + _config.retreatWeight + _config.idleWeight;
        float roll = Random.Range(0f, total);

        if (roll < _config.blockWeight) return AIState.Block;
        roll -= _config.blockWeight;

        if (roll < _config.retreatWeight) return AIState.Retreat;

        return AIState.Idle;
    }

    // -------- Reaction Delay & Transition --------

    public void ScheduleTransition(AIState nextState)
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
        _currentState.Exit();
        _currentAIState = nextState;
        _currentState = _statesMap[nextState];
        _currentState.Enter();
        Debug.Log($"[BotBrain] → {nextState}");
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
    }

    public void SetConfig(BotDifficultyConfig config)
    {
        _config = config;
        if (_sensor != null)
        {
            _sensor.Init(config, _playerHitBoxTag);
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
