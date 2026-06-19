using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BotInput))]
public class BotBrain : MonoBehaviour, IBotContext
{
    public enum AIState { Idle, Approach, Attack, Block, Retreat, Charge }

    [Header("Target")]
    [SerializeField] private Transform _target;

    [Header("Difficulty")]
    [SerializeField] private BotDifficultyConfig _config;

    [Header("Threat Detection")]
    [SerializeField] private LayerMask _playerHitBoxMask;

    [Header("Tick Performance")]
    [SerializeField] private float _thinkInterval = 0.05f; // 20Hz

    [Header("Debug Info")]
    [SerializeField] private AIState _currentAIState = AIState.Idle;

    public event System.Action<AIState, AIState> OnStateChanged;
    public event System.Action OnTargetLost;
    public event System.Action<Transform> OnTargetAcquired;
    public event System.Action<float> OnDamageTaken;

    private BotSensor _sensor;
    private BotExecutor _executor;
    private PlayerPatternTracker _patternTracker;
    private Dictionary<AIState, BotState> _statesMap;
    private BotState _currentState;

    private float _attackTimer;
    private float _reactionTimer;
    private readonly Queue<AIState> _pendingStates = new Queue<AIState>();

    private float _thinkTimer;
    private float _accumulatedDeltaTime;
    private IBotDecisionPolicy _decisionPolicy;
    private HealthManager _healthManager;
    private float _lastHealthValue;
    private IManaChecker _mana;
    private SkillController _skillController;

    public BotDifficultyConfig Config => _config;
    public IBotSensor Sensor => _sensor;
    public IBotExecutor Executor => _executor;
    public float AttackTimer => _attackTimer;
    public bool IsTransitionPending => _pendingStates.Count > 0;
    public Transform Target => _target;
    public IManaChecker Mana => _mana;
    public SkillController SkillController => _skillController;

    private void Awake()
    {
        _sensor = GetComponent<BotSensor>() ?? gameObject.AddComponent<BotSensor>();
        _sensor.Init(_config, _playerHitBoxMask);

        _executor = GetComponent<BotExecutor>() ?? gameObject.AddComponent<BotExecutor>();
        _executor.Init();

        _patternTracker = GetComponent<PlayerPatternTracker>() ?? gameObject.AddComponent<PlayerPatternTracker>();
        _patternTracker.Init(_config, _target);

        // Khởi tạo chính sách quyết định dựa theo cấu hình
        if (_config != null && _config.Pattern.enablePatternTracking)
        {
            _decisionPolicy = new PatternAdaptivePolicy();
        }
        else
        {
            _decisionPolicy = new WeightedRandomPolicy();
        }

        // Đăng ký theo dõi sự kiện thay đổi máu để bắt sát thương
        _healthManager = GetComponent<HealthManager>();
        if (_healthManager != null)
        {
            _lastHealthValue = _healthManager.CurrentHealth;
            _healthManager.OnChangeHealth += HandleHealthChanged;
        }

        _mana = GetComponent<IManaChecker>();
        _skillController = GetComponent<SkillController>();

        _statesMap = new Dictionary<AIState, BotState>
        {
            { AIState.Idle, new BotIdleState(this) },
            { AIState.Approach, new BotApproachState(this, _patternTracker) },
            { AIState.Attack, new BotAttackState(this) },
            { AIState.Block, new BotBlockState(this) },
            { AIState.Retreat, new BotRetreatState(this) },
            { AIState.Charge, new BotChargeState(this) }
        };

        _thinkTimer = Random.Range(0f, _thinkInterval); // Phân tán điểm khởi đầu tick
    }

    private void OnDestroy()
    {
        if (_healthManager != null)
        {
            _healthManager.OnChangeHealth -= HandleHealthChanged;
        }
    }

    private void Start()
    {
        _config?.ValidateValues();
        if (_target != null)
        {
            _sensor.SetTarget(_target);
            _patternTracker.SetTarget(_target);
        }
        _currentAIState = AIState.Idle;
        _currentState = _statesMap[_currentAIState];
        _currentState.Enter();
    }

    private void Update()
    {
        _executor.ClearAttackInput();

        UpdateTimers();
        ProcessPendingTransitions();

        _accumulatedDeltaTime += Time.unscaledDeltaTime;

        // Tần suất suy nghĩ của AI (Rate-limited, chạy kể cả khi target = null để sensor có thể quét tìm Player)
        _thinkTimer -= Time.unscaledDeltaTime;
        if (_thinkTimer <= 0)
        {
            TickAI();
        }

        UpdateActiveState();
    }

    private void UpdateTimers()
    {
        _attackTimer -= Time.unscaledDeltaTime;
        _reactionTimer -= Time.unscaledDeltaTime;
    }

    private void ProcessPendingTransitions()
    {
        if (_pendingStates.Count > 0 && _reactionTimer <= 0)
        {
            AIState nextState = _pendingStates.Dequeue();
            ExecuteTransition(nextState);
            if (_pendingStates.Count > 0)
            {
                ResetReactionTimer(_pendingStates.Peek());
            }
        }
    }

    private void TickAI()
    {
        _thinkTimer = _thinkInterval;

        if (_sensor != null)
        {
            _sensor.UpdateSensor();

            if (_target == null && _sensor.Target != null)
            {
                SetTarget(_sensor.Target);
            }
        }

        // Nếu vẫn chưa có target hoặc config, dừng xử lý FSM/PatternTracker của Tick này
        if (_target == null || _config == null) return;

        if (_config.Pattern.enablePatternTracking)
        {
            _patternTracker.Tick(_accumulatedDeltaTime);
        }
        _accumulatedDeltaTime = 0f;

        if (_sensor.ThreatDetected)
        {
            AIState reaction = _decisionPolicy.PickThreatReaction(_config, _patternTracker);
            if (reaction == AIState.Block || reaction == AIState.Retreat)
            {
                ScheduleTransition(reaction);
            }
        }
    }

    private void UpdateActiveState()
    {
        // Nếu vẫn chưa có target hoặc config, không chạy FSM Update của Frame này
        if (_target == null || _config == null) return;

        // Cập nhật trạng thái hiện tại (FSM) chạy mỗi frame để đảm bảo di chuyển mượt và timer chém đòn chính xác
        _currentState.Update();
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        if (currentHealth < _lastHealthValue)
        {
            float damage = _lastHealthValue - currentHealth;
            OnDamageTaken?.Invoke(damage);
        }
        _lastHealthValue = currentHealth;
    }

    public void ScheduleTransition(AIState nextState)
    {
        // Đảm bảo trạng thái hiện tại đồng ý cho phép ngắt
        if (_currentState != null && !_currentState.CanInterrupt(nextState)) return;

        // Cơ chế ghi đè khẩn cấp: nếu chuyển sang trạng thái phòng thủ khẩn cấp (Block/Retreat)
        bool isEmergency = nextState == AIState.Block || nextState == AIState.Retreat;
        if (isEmergency)
        {
            // Clear các trạng thái không phải khẩn cấp đang chờ trong queue
            if (_pendingStates.Count > 0)
            {
                AIState currentPending = _pendingStates.Peek();
                if (currentPending != AIState.Block && currentPending != AIState.Retreat)
                {
                    _pendingStates.Clear();
                }
            }
        }

        // Tránh xếp hàng trùng lặp liên tục cùng một trạng thái
        if (_pendingStates.Contains(nextState)) return;

        _pendingStates.Enqueue(nextState);

        // Nếu đây là trạng thái đầu tiên hoặc vừa bị clear và làm mới, bắt đầu tính giờ phản xạ
        if (_pendingStates.Count == 1)
        {
            ResetReactionTimer(nextState);
        }
    }

    private void ResetReactionTimer(AIState state)
    {
        _reactionTimer = _config.Timing.reactionDelay * (state == AIState.Block || state == AIState.Retreat ? 0.25f : 1f);
    }



    private void ExecuteTransition(AIState nextState)
    {
        AIState prevState = _currentAIState;
        _currentState.Exit();
        _currentAIState = nextState;
        _currentState = _statesMap[nextState];
        _currentState.Enter();
        Debug.Log($"[BotBrain] {gameObject.name} chuyển trạng thái: {prevState} -> {nextState}");
        OnStateChanged?.Invoke(prevState, nextState);
    }

    public void SetAttackCooldown() => _attackTimer = _config.Timing.attackCooldown;

    public void SetTarget(Transform target)
    {
        Transform prevTarget = _target;
        _target = target;
        _sensor?.SetTarget(target);
        _patternTracker?.SetTarget(target);

        if (prevTarget != null && target == null)
        {
            OnTargetLost?.Invoke();
        }
        else if (prevTarget == null && target != null)
        {
            OnTargetAcquired?.Invoke(target);
        }
    }

    public void SetConfig(BotDifficultyConfig config)
    {
        _config = config;
        _config?.ValidateValues();
        _sensor?.Init(config, _playerHitBoxMask);
        _patternTracker?.Init(config, _target);

        if (config != null && config.Pattern.enablePatternTracking)
        {
            _decisionPolicy = new PatternAdaptivePolicy();
        }
        else
        {
            _decisionPolicy = new WeightedRandomPolicy();
        }

        _pendingStates.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_config == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _config.Detection.detectRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _config.Detection.threatRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _config.Detection.attackRange);
    }
#endif
}
