using UnityEngine;

public class FlashSkill : Skill
{
    private Rigidbody2D _rigidbody2D;
    private Collider2D[] _colliders;
    private Vector2 _dashVelocity;
    private float _dashTimer;
    private bool _isDashing;

    private const float DASH_DURATION = 0.5f; // 1 giây lướt

    public override void Init(GameObject owner, BaseSkill config)
    {
        base.Init(owner, config);
        _rigidbody2D = owner.GetComponent<Rigidbody2D>();
        _colliders = owner.GetComponentsInChildren<Collider2D>();
    }

    protected override void ExecuteLogic()
    {
        if (_rigidbody2D == null) return;

        // Ép kiểu cấu hình
        var dashConfig = Config as DashSkillData;
        float distance = dashConfig != null ? dashConfig.dashDistance : 5f;

        // Xác định hướng nhìn ban đầu kết hợp hướng xoay và localScale để tương thích cả 2 bộ xoay
        float facingSign = Mathf.Sign(Owner.transform.right.x) * Mathf.Sign(Owner.transform.localScale.x);
        Vector2 dashDir = new Vector2(facingSign, 0f);

        // Vận tốc = Khoảng cách / Thời gian
        _dashVelocity = dashDir * (distance / DASH_DURATION);
        _dashTimer = DASH_DURATION;
        _isDashing = true;

        // Tạm thời bỏ qua va chạm vật lý với tất cả nhân vật khác để lướt xuyên qua được
        SetCharacterCollisions(true);
    }

    public override void OnUpdate(float deltaTime)
    {
        if (!_isDashing || _rigidbody2D == null) return;

        // Áp dụng vận tốc lướt liên tục
        _rigidbody2D.linearVelocity = _dashVelocity;

        _dashTimer -= deltaTime;
        if (_dashTimer <= 0)
        {
            StopDash();
        }
    }

    private void StopDash()
    {
        if (!_isDashing) return;
        _isDashing = false;

        if (_rigidbody2D != null)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
        }

        // Khôi phục lại va chạm vật lý với các nhân vật khác
        SetCharacterCollisions(false);

        // Tự động gọi dừng skill để SkillController chuyển IsExecuting về false
        var controller = Owner.GetComponent<SkillController>();
        if (controller != null)
        {
            controller.StopSkill(Config.skillType);
        }
    }

    private void SetCharacterCollisions(bool ignore)
    {
        if (_colliders == null || _colliders.Length == 0) return;

        // Lấy đối thủ trực tiếp qua bộ xoay (RotationHandler) thay vì quét toàn bộ Scene
        var rotationHandler = Owner.GetComponent<RotationHandler>();
        if (rotationHandler != null && rotationHandler.Target != null)
        {
            var otherColliders = rotationHandler.Target.GetComponentsInChildren<Collider2D>();
            for (int j = 0; j < otherColliders.Length; j++)
            {
                for (int k = 0; k < _colliders.Length; k++)
                {
                    if (_colliders[k] != null && otherColliders[j] != null)
                    {
                        Physics2D.IgnoreCollision(_colliders[k], otherColliders[j], ignore);
                    }
                }
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        StopDash();
    }
}
