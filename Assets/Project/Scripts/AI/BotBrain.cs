using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BotInput))]
public class BotBrain : MonoBehaviour, IBotContext
{
    public enum AIState { Idle, Approach, Attack, Block, Retreat }

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
    private AIState? _pendingState;

    private float _thinkTimer;
    private float _accumulatedDeltaTime;
    private IBotDecisionPolicy _decisionPolicy;
    private HealthManager _healthManager;
    private float _lastHealthValue;

    public BotDifficultyConfig Config => _config;
    public IBotSensor Sensor => _sensor;
    public IBotExecutor Executor => _executor;
    public PlayerPatternTracker PatternTracker => _patternTracker;
    public float AttackTimer => _attackTimer;
    public bool IsTransitionPending => _pendingState.HasValue;
    public Transform Target => _target;

    private void Awake()
    {
        _sensor = GetComponent<BotSensor>() ?? gameObject.AddComponent<BotSensor>();
        _sensor.Init(_config, _playerHitBoxMask);

        _executor = GetComponent<BotExecutor>() ?? gameObject.AddComponent<BotExecutor>();
        _executor.Init();

        _patternTracker = GetComponent<PlayerPatternTracker>() ?? gameObject.AddComponent<PlayerPatternTracker>();
        _patternTracker.Init(_config, _target);

        // Khởi tạo chính sách quyết định dựa theo cấu hình
        if (_config != null && _config.enablePatternTracking)
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

        _statesMap = new Dictionary<AIState, BotState>
        {
            { AIState.Idle, new BotIdleState(this) },
            { AIState.Approach, new BotApproachState(this) },
            { AIState.Attack, new BotAttackState(this) },
            { AIState.Block, new BotBlockState(this) },
            { AIState.Retreat, new BotRetreatState(this) }
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

        // Giảm trừ bộ đếm thời gian thực (unscaled) độc lập với Time.timeScale
        _attackTimer -= Time.unscaledDeltaTime;
        _reactionTimer -= Time.unscaledDeltaTime;

        // Chuyển trạng thái tức thời khi hết thời gian phản xạ (Reaction Delay)
        if (_pendingState.HasValue && _reactionTimer <= 0)
        {
            ExecuteTransition(_pendingState.Value);
            _pendingState = null;
        }

        _accumulatedDeltaTime += Time.unscaledDeltaTime;

        // Tần suất suy nghĩ của AI (Rate-limited, chạy kể cả khi target = null để sensor có thể quét tìm Player)
        _thinkTimer -= Time.unscaledDeltaTime;
        if (_thinkTimer <= 0)
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

            if (_config.enablePatternTracking)
            {
                _patternTracker.Tick(_accumulatedDeltaTime);
            }
            _accumulatedDeltaTime = 0f;

            if (_sensor.ThreatDetected && _currentAIState != AIState.Block && _currentAIState != AIState.Retreat && !_pendingState.HasValue)
            {
                ScheduleTransition(PickThreatReaction());
            }
        }

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

    public AIState PickAttackAction()
    {
        if (_decisionPolicy == null) return AIState.Idle;
        return _decisionPolicy.PickAttackAction(_config, _patternTracker);
    }

    public AIState PickThreatReaction()
    {
        if (_decisionPolicy == null) return AIState.Idle;
        return _decisionPolicy.PickThreatReaction(_config, _patternTracker);
    }

    public void ScheduleTransition(AIState nextState)
    {
        if (_pendingState == nextState) return;

        // Đảm bảo trạng thái hiện tại đồng ý cho phép ngắt đè đòn
        if (_currentState != null && !_currentState.CanInterrupt(nextState)) return;

        if (_pendingState.HasValue && GetStatePriority(nextState) < GetStatePriority(_pendingState.Value)) return;

        _pendingState = nextState;
        _reactionTimer = _config.reactionDelay * (nextState == AIState.Block || nextState == AIState.Retreat ? 0.25f : 1f);
    }

    private int GetStatePriority(AIState state)
    {
        if (_statesMap.TryGetValue(state, out var botState))
        {
            return botState.Priority;
        }
        return 0;
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

    public void SetAttackCooldown() => _attackTimer = _config.attackCooldown;

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
        _sensor?.Init(config, _playerHitBoxMask);
        _patternTracker?.Init(config, _target);

        if (config != null && config.enablePatternTracking)
        {
            _decisionPolicy = new PatternAdaptivePolicy();
        }
        else
        {
            _decisionPolicy = new WeightedRandomPolicy();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_config == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _config.detectRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _config.threatRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _config.attackRange);
    }
#endif
}
