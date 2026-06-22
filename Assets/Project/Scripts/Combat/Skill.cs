using UnityEngine;

public abstract class Skill
{
    public GameObject Owner { get; protected set; }
    public BaseSkill Config { get; protected set; }
    public float CooldownTimer { get; protected set; }
    public bool IsExecuting { get; protected set; } // Trạng thái đang thi triển chiêu

    protected IManaChecker _manaChecker;
    protected IManaConsumer _manaConsumer;

    public virtual void Init(GameObject owner, BaseSkill config)
    {
        Owner = owner;
        Config = config;
        CooldownTimer = 0f;
        IsExecuting = false;
        _manaChecker = owner.GetComponent<IManaChecker>();
        _manaConsumer = owner.GetComponent<IManaConsumer>();
    }

    public virtual void UpdateCooldown(float deltaTime)
    {
        if (CooldownTimer > 0f)
            CooldownTimer -= deltaTime;
    }

    public virtual bool CanCast()
    {
        return CooldownTimer <= 0f && (_manaChecker == null || _manaChecker.HasManaReached(Config.manaCost));
    }

    public virtual void Cast()
    {
        _manaConsumer?.ConsumeMana(Config.manaCost);
        CooldownTimer = Config.cooldown;
        IsExecuting = true;
        TriggerAnimation(true);
        ExecuteLogic();
    }

    protected virtual void TriggerAnimation(bool start)
    {
        if (string.IsNullOrEmpty(Config.animationBoolOrTriggerName)) return;
        var animator = Owner.GetComponent<Animator>();
        if (animator == null) return;

        // Kiểm tra xem parameter có tồn tại trong Animator của nhân vật không để tránh warning spam
        if (!HasParameter(animator, Config.animationBoolOrTriggerName, out AnimatorControllerParameterType type))
            return;

        if (Config.isContinuous)
        {
            if (type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(Config.animationBoolOrTriggerName, start);
            }
        }
        else if (start) // Instant skill chỉ kích hoạt trigger khi bắt đầu
        {
            if (type == AnimatorControllerParameterType.Trigger)
            {
                animator.SetTrigger(Config.animationBoolOrTriggerName);
            }
        }
    }

    private bool HasParameter(Animator animator, string paramName, out AnimatorControllerParameterType type)
    {
        type = AnimatorControllerParameterType.Bool;
        if (animator.parameterCount == 0) return false;

        var parameters = animator.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].name == paramName)
            {
                type = parameters[i].type;
                return true;
            }
        }
        return false;
    }

    protected abstract void ExecuteLogic();
    public virtual void OnUpdate(float deltaTime) {}
    
    public virtual void OnEnd() 
    {
        IsExecuting = false;
        TriggerAnimation(false);
    }
}
