using UnityEngine;

public class FlashSkill : Skill
{
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider;
    private Vector2 _dashVelocity;
    private float _dashTimer;
    private bool _isDashing;
    private Collider2D _ignoredTargetCollider;

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

        // Lấy hướng di chuyển hiện tại từ input của nhân vật
        var charInput = Owner.GetComponent<CharacterInput>();
        Vector2 inputDir = (charInput != null && charInput.MovementInput != null) 
            ? charInput.MovementInput.ProvideMovementInput() 
            : Vector2.zero;

        Vector2 dashDir;
        if (inputDir != Vector2.zero)
        {
            dashDir = inputDir.normalized;
        }
        else
        {
            // Dự phòng: Lấy VisualRoot của nhân vật để xác định hướng nhìn thực tế của hình ảnh trực quan
            var character = Owner.GetComponent<Character>();
            Transform visualTransform = (character != null && character.VisualRoot != null) 
                ? character.VisualRoot.transform 
                : Owner.transform;

            // Xác định hướng nhìn 2D thực tế dựa trên trục quay và localScale của VisualRoot
            dashDir = (Vector2)(visualTransform.right * Mathf.Sign(visualTransform.localScale.x));
        }

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

        if (ignore)
        {
            // Lấy đối thủ trực tiếp qua bộ xoay (RotationHandler) thay vì quét toàn bộ Scene
            var rotationHandler = Owner.GetComponent<RotationHandler>();
            if (rotationHandler != null && rotationHandler.Target != null)
            {
                _ignoredTargetCollider = rotationHandler.Target.GetComponentInChildren<Collider2D>();
                if (_ignoredTargetCollider != null)
                {
                    Physics2D.IgnoreCollision(_collider, _ignoredTargetCollider, true);
                }
            }
        }
        else
        {
            if (_ignoredTargetCollider != null)
            {
                Physics2D.IgnoreCollision(_collider, _ignoredTargetCollider, false);
                _ignoredTargetCollider = null;
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        StopDash();
    }
}
