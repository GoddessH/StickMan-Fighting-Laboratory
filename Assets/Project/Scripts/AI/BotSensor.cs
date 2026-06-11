using UnityEngine;

public class BotSensor : MonoBehaviour
{
    [SerializeField] private string _playerHitBoxTag = "PlayerHitBox";

    private Transform _target;
    private BotDifficultyConfig _config;

    public Transform Target => _target;
    public float Distance { get; private set; }
    public float DistanceY { get; private set; }
    public Vector2 DirectionToTarget { get; private set; }
    public bool ThreatDetected { get; private set; }

    public void Init(BotDifficultyConfig config, string playerHitBoxTag)
    {
        _config = config;
        _playerHitBoxTag = playerHitBoxTag;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void UpdateSensor()
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

        if (_target == null || _config == null)
        {
            Distance = 0f;
            DistanceY = 0f;
            DirectionToTarget = Vector2.zero;
            ThreatDetected = false;
            return;
        }

        Vector2 toTarget = (Vector2)(_target.position - transform.position);
        Distance = toTarget.magnitude;
        DistanceY = _target.position.y - transform.position.y;
        DirectionToTarget = toTarget.normalized;

        ThreatDetected = DetectPlayerHitBox();
    }

    private bool DetectPlayerHitBox()
    {
        if (_target != null)
        {
            StateController targetStateController = _target.GetComponent<StateController>();
            if (targetStateController != null && targetStateController.CurrentStateType == StateType.Attack)
            {
                if (Distance <= _config.threatRange)
                {
                    return true;
                }
            }
        }

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
}
