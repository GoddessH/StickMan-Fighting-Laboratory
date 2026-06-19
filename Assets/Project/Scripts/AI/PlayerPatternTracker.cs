using UnityEngine;

public class PlayerPatternTracker : MonoBehaviour
{
    private Transform _target;
    private StateController _targetStateController;
    private AnimationHandler _targetAnimationHandler;
    private BotDifficultyConfig _config;

    // Chỉ số EMA công khai
    public float AggressionScore { get; private set; }
    public float DefensivenessScore { get; private set; }
    public float AirborneScore { get; private set; }

    private bool _isTrackingEnabled = true;

    public void Init(BotDifficultyConfig config, Transform target)
    {
        _config = config;
        SetTarget(target);

        // Khởi tạo các điểm số về 0
        AggressionScore = 0f;
        DefensivenessScore = 0f;
        AirborneScore = 0f;
        _isTrackingEnabled = true;
    }

    public void SetTarget(Transform target)
    {
        if (_target != target)
        {
            _target = target;
            if (target != null)
            {
                _targetStateController = target.GetComponent<StateController>();
                _targetAnimationHandler = target.GetComponentInChildren<AnimationHandler>();
            }
            else
            {
                _targetStateController = null;
                _targetAnimationHandler = null;
            }
        }
    }

    public void EnableTracking() => _isTrackingEnabled = true;
    public void DisableTracking() => _isTrackingEnabled = false;

    public void Tick(float deltaTime)
    {
        if (_target == null || _config == null || !_config.Pattern.enablePatternTracking || !_isTrackingEnabled)
        {
            return;
        }

        // Tự động tìm lại target nếu bị mất refs
        if (_targetStateController == null)
        {
            _targetStateController = _target.GetComponent<StateController>();
        }
        if (_targetAnimationHandler == null)
        {
            _targetAnimationHandler = _target.GetComponentInChildren<AnimationHandler>();
        }

        // Tính alpha dựa trên half-life (thời gian bán rã để quên lối chơi cũ)
        float halfLife = _config.Pattern.patternTrackingHalfLife;
        if (halfLife <= 0f) halfLife = 3.0f;
        float lambda = Mathf.Log(2f) / halfLife;
        float alpha = 1f - Mathf.Exp(-deltaTime * lambda);
        alpha = Mathf.Clamp01(alpha);

        // 1. Theo dõi Tấn công (Aggression)
        float isAttacking = 0f;
        if (_targetStateController != null && _targetStateController.CurrentStateType == StateType.Attack)
        {
            isAttacking = 1f;
        }
        AggressionScore = Mathf.Lerp(AggressionScore, isAttacking, alpha);

        // 2. Theo dõi Phòng thủ (Defensiveness)
        float isBlocking = 0f;
        if (_targetStateController != null && _targetStateController.CurrentStateType == StateType.Block)
        {
            isBlocking = 1f;
        }
        DefensivenessScore = Mathf.Lerp(DefensivenessScore, isBlocking, alpha);

        // 3. Theo dõi Bay nhảy (Airborne)
        float isAirborne = 0f;
        if (_targetAnimationHandler != null)
        {
            bool flying = _targetAnimationHandler.GetBool(AnimationName.Fly);
            bool falling = _targetAnimationHandler.GetBool(AnimationName.Fall);
            if (flying || falling)
            {
                isAirborne = 1f;
            }
        }
        else
        {
            // Trình fallback nếu không tìm thấy AnimationHandler
            float diffY = _target.position.y - transform.position.y;
            if (Mathf.Abs(diffY) > _config.Movement.flyThreshold)
            {
                isAirborne = 1f;
            }
        }
        AirborneScore = Mathf.Lerp(AirborneScore, isAirborne, alpha);
    }
}
