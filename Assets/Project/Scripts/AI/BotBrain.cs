using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BotInput))]
public class BotBrain : MonoBehaviour
{
    public enum AIState { Idle, Approach, Attack, Block, Retreat }

    [Header("Target")]
    [SerializeField] private Transform _target;

    [Header("Difficulty")]
    [SerializeField] private BotDifficultyConfig _config;

    [Header("Threat Detection")]
    [SerializeField] private LayerMask _playerHitBoxMask;

    [Header("Debug Info")]
    [SerializeField] private AIState _currentAIState = AIState.Idle;

    public event System.Action<AIState, AIState> OnStateChanged;

    private BotSensor _sensor;
    private BotExecutor _executor;
    private PlayerPatternTracker _patternTracker;
    private Dictionary<AIState, BotState> _statesMap;
    private BotState _currentState;

    private float _attackTimer;
    private float _reactionTimer;
    private AIState? _pendingState;

    public BotDifficultyConfig Config => _config;
    public float AttackTimer => _attackTimer;
    public bool IsTransitionPending => _pendingState.HasValue;
    public PlayerPatternTracker PatternTracker => _patternTracker;

    private void Awake()
    {
        _sensor = GetComponent<BotSensor>() ?? gameObject.AddComponent<BotSensor>();
        _sensor.Init(_config, _playerHitBoxMask);

        _executor = GetComponent<BotExecutor>() ?? gameObject.AddComponent<BotExecutor>();
        _executor.Init();

        _patternTracker = GetComponent<PlayerPatternTracker>() ?? gameObject.AddComponent<PlayerPatternTracker>();
        _patternTracker.Init(_config, _target);

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
            _patternTracker.SetTarget(_target);
        }
        _currentAIState = AIState.Idle;
        _currentState = _statesMap[_currentAIState];
        _currentState.Enter();
    }

    private void Update()
    {
        if (_target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) SetTarget(player.transform);
        }

        _executor.ClearAttackInput();
        if (_target == null || _config == null) return;

        _sensor.UpdateSensor();
        _attackTimer -= Time.deltaTime;
        _reactionTimer -= Time.deltaTime;

        if (_pendingState.HasValue && _reactionTimer <= 0)
        {
            ExecuteTransition(_pendingState.Value);
            _pendingState = null;
        }

        if (_sensor.ThreatDetected && _currentAIState != AIState.Block && !_pendingState.HasValue)
        {
            ScheduleTransition(PickThreatReaction());
            return;
        }

        _currentState.Update();
    }

    public AIState PickAttackAction() => _config.PickAttackAction(_patternTracker);
    public AIState PickThreatReaction() => _config.PickThreatReaction(_patternTracker);

    public void ScheduleTransition(AIState nextState)
    {
        if (_pendingState == nextState) return;
        if (_pendingState.HasValue && GetStatePriority(nextState) < GetStatePriority(_pendingState.Value)) return;

        _pendingState = nextState;
        _reactionTimer = _config.reactionDelay * (nextState == AIState.Block || nextState == AIState.Retreat ? 0.25f : 1f);
    }

    private int GetStatePriority(AIState state) =>
        state == AIState.Block || state == AIState.Retreat ? 2 : (state == AIState.Attack ? 1 : 0);

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
        _target = target;
        _sensor?.SetTarget(target);
        _patternTracker?.SetTarget(target);
    }

    public void SetConfig(BotDifficultyConfig config)
    {
        _config = config;
        _sensor?.Init(config, _playerHitBoxMask);
        _patternTracker?.Init(config, _target);
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
