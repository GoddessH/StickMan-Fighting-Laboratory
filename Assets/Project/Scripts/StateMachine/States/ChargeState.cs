using Photon.Pun;
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
        SpawnerManager.Instance.VFXSpawner.SpawnChargeAura(_ownerGO.GetComponent<PhotonView>().ViewID);
        _tick = 0;
    }

    public override void UpdateState()
    {
        if (_onCheckHolding == null || !_onCheckHolding.Invoke() || _manaChecker != null && _manaChecker.IsFullMana())
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
        SpawnerManager.Instance.VFXSpawner.DestroyChargeAura();
    }
    #endregion
}
