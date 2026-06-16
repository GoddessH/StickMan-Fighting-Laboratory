using UnityEngine;

public class FlashSkill : Skill
{
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider;
    private Vector2 _dashVelocity;
    private float _dashTimer;
    private bool _isDashing;

    private const float DASH_DURATION = 0.2f; // 1 giây lướt

    public override void Init(GameObject owner, BaseSkill config)
    {
        base.Init(owner, config);
        _rigidbody2D = owner.GetComponent<Rigidbody2D>();
        _collider = owner.GetComponentInChildren<Collider2D>();
    }

    protected override void ExecuteLogic()
    {
        if (_rigidbody2D == null) return;

        // Ép kiểu cấu hình
        var dashConfig = Config as DashSkillData;
        float distance = dashConfig != null ? dashConfig.dashDistance : 5f;

        // Xác định hướng nhìn 2D thực tế dựa trên trục quay và localScale
        Vector2 dashDir = (Vector2)(Owner.transform.right * Mathf.Sign(Owner.transform.localScale.x));

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
        if (_collider == null) return;

        // Lấy đối thủ trực tiếp qua bộ xoay (RotationHandler) thay vì quét toàn bộ Scene
        var rotationHandler = Owner.GetComponent<RotationHandler>();
        if (rotationHandler != null && rotationHandler.Target != null)
        {
            var otherCollider = rotationHandler.Target.GetComponentInChildren<Collider2D>();
            if (otherCollider != null)
            {
                Physics2D.IgnoreCollision(_collider, otherCollider, ignore);
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        StopDash();
    }
}
