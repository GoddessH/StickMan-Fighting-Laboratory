using System;
using UnityEngine;

public class ChargeState : State
{
    //
    private Func<bool> _onCheckHolding;
    private IManaRegenerator _manaRegenerator;
    private IManaChecker _manaChecker;
    private float _tick;

    #region Implement State
    protected override void SetContext()
    {
        _onCheckHolding = _ownerGO.GetComponent<ChargeController>()?.Provide();
        _manaRegenerator = _ownerGO.GetComponent<IManaRegenerator>();
        _manaChecker = _ownerGO.GetComponent<IManaChecker>();
    }

    public override void EnterState()
    {
        _stateData.AnimationHandler.SetBool(AnimationName.Idle, true);
        VFXSpawner.Instance.SpawnChargeAura(_ownerGO.transform);
        _tick = 0;
    }

    public override void UpdateState()
    {
        if (_onCheckHolding == null || !_onCheckHolding.Invoke() || _manaChecker != null && _manaChecker.CheckFullMana())
        {
            _onComplete?.Invoke(_type);
            return;
        }

        _tick += Time.deltaTime;
        if (_tick >= 1)
        {
            _manaRegenerator?.RegenerateMana();
            _tick -= 1;
        }
    }

    public override void ExitState()
    {
        _stateData.AnimationHandler.SetBool(AnimationName.Idle, false);
        _tick = 0;
        VFXSpawner.Instance.SpawnChargeAura(isDestroy: true);
    }
    #endregion
}
