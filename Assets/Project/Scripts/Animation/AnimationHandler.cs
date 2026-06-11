using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]
public class AnimationHandler : MonoBehaviour
{
    // 
    private Animator _animator;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected void LocalSetBool(string flag, bool value)
        => _animator.SetBool(flag, value);

    protected void LocalSetInteger(string parameter, int value)
        => _animator.SetInteger(parameter, value);

    public bool GetBool(string flag)
        => _animator.GetBool(flag);

    public virtual void SetBool(string flag, bool value)
        => LocalSetBool(flag, value);

    public virtual void SetInteger(string parameter, int value)
        => LocalSetInteger(parameter, value);

    public (bool, float) CheckCurrentState(string stateName)
    {
        AnimatorStateInfo currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return (currentStateInfo.IsName(stateName), currentStateInfo.normalizedTime);
    }
}
