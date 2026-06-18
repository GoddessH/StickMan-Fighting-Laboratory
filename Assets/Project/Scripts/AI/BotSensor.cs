using UnityEngine;

public class BotSensor : MonoBehaviour, IBotSensor
{
    [SerializeField] private LayerMask _playerHitBoxMask;

    private Transform _target;
    private BotDifficultyConfig _config;
    private StateController _targetStateController;

    private static readonly Collider2D[] s_colliderBuffer = new Collider2D[1];

    private float _lastTargetSearchTime = -10f;

    public Transform Target => _target;
    public float Distance { get; private set; }
    public float DistanceX { get; private set; }
    public float DistanceY { get; private set; }
    public Vector2 DirectionToTarget { get; private set; }
    public bool ThreatDetected { get; private set; }

    public void Init(BotDifficultyConfig config, LayerMask playerHitBoxMask)
    {
        _config = config;
        _playerHitBoxMask = playerHitBoxMask;
    }

    public void SetTarget(Transform target)
    {
        if (_target != target)
        {
            _target = target;
            _targetStateController = target != null ? target.GetComponent<StateController>() : null;
        }
    }

    public void UpdateSensor()
    {
        // Tự động tìm Player nếu chưa được gán (Rate-limited)
        if (_target == null)
        {
            if (Time.time - _lastTargetSearchTime >= 0.5f)
            {
                _lastTargetSearchTime = Time.time;
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    SetTarget(player.transform);
                }
            }
        }

        if (_target == null || _config == null)
        {
            Distance = 0f;
            DistanceX = 0f;
            DistanceY = 0f;
            DirectionToTarget = Vector2.zero;
            ThreatDetected = false;
            return;
        }

        Vector2 toTarget = (Vector2)(_target.position - transform.position);
        Distance = toTarget.magnitude;
        DistanceX = Mathf.Abs(toTarget.x);
        DistanceY = _target.position.y - transform.position.y;
        DirectionToTarget = toTarget.normalized;

        ThreatDetected = DetectPlayerHitBox();
    }

    private bool DetectPlayerHitBox()
    {
        if (_target != null)
        {
            if (_targetStateController == null)
            {
                _targetStateController = _target.GetComponent<StateController>();
            }

            if (_targetStateController != null && _targetStateController.CurrentStateType == StateType.Attack)
            {
                if (Distance <= _config.threatRange)
                {
                    return true;
                }
            }
        }

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = _playerHitBoxMask;
        contactFilter.useTriggers = true;

        int count = Physics2D.OverlapCircle(
            transform.position,
            _config.threatRange,
            contactFilter,
            s_colliderBuffer
        );

        bool threatDetected = count > 0;

        // BẮT BUỘC giải phóng các phần tử trong buffer tĩnh về null sau khi xử lý xong
        for (int i = 0; i < s_colliderBuffer.Length; i++)
        {
            s_colliderBuffer[i] = null;
        }

        return threatDetected;
    }
}
