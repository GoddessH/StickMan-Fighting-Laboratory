using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationHandler : MonoBehaviour
{
    // 
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public bool GetBool(string flag)
        => _animator.GetBool(flag);

    public void SetBool(string flag, bool value)
        => _animator?.SetBool(flag, value);

    public void SetInteger(string parameter, int value)
        => _animator.SetInteger(parameter, value);

    public (bool, float) CheckCurrentState(string stateName)
    {
        AnimatorStateInfo currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return (currentStateInfo.IsName(stateName), currentStateInfo.normalizedTime);
    }
}
