using System;
using UnityEngine;

public class BlockState : State
{
    // Delegate từ BlockController — kiểm tra "đang giữ nút Block không?"
    private Func<bool> _onCheckBlockHolding;
    // Delegate từ AttackController — kiểm tra "vừa bấm Attack không?"
    private Func<bool> _onCheckAttackInput;

    private BlockController _blockController;
    private SkillController _skillController;

    private float _blockTimer;

    protected override void SetContext()
    {
        _blockController = _ownerGO.GetComponent<BlockController>();
        _onCheckBlockHolding = _blockController?.Provide();
        _onCheckAttackInput = _ownerGO.GetComponent<AttackController>()?.Provide();
        _skillController = _ownerGO.GetComponent<SkillController>();
    }

    public override void EnterState()
    {
        if (_onCheckBlockHolding == null)
        {
            _onComplete?.Invoke(_type);
            return;
        }

        if (_skillController != null && !_skillController.CanCast(SkillType.Block))
        {
            _onComplete?.Invoke(_type);
            return;
        }

        // Bật animation Block
        _stateData.AnimationHandler.SetBool(AnimationName.Block, true);

        // Bật trạng thái Block trong Controller
        _blockController?.SetBlocking(true);

        _skillController?.CastSkill(SkillType.Block);
        _blockTimer = 5.0f;
    }

    public override void UpdateState()
    {
        // Thoát Block nếu thả nút
        if (_onCheckBlockHolding != null && !_onCheckBlockHolding.Invoke())
        {
            _onComplete?.Invoke(_type);
            return;
        }

        // Cancel Block để tấn công (Block → Attack)
        if (_onCheckAttackInput != null && _onCheckAttackInput.Invoke())
        {
            _onComplete?.Invoke(_type);
            return;
        }

        // Đếm ngược giới hạn 5s
        _blockTimer -= Time.deltaTime;
        if (_blockTimer <= 0)
        {
            _onComplete?.Invoke(_type);
            return;
        }

        // Tiêu hao mana duy trì
        if (_skillController != null)
        {
            if (!_skillController.ConsumeContinuousMana(SkillType.Block, Time.deltaTime))
            {
                _onComplete?.Invoke(_type);
                return;
            }
        }
    }

    public override void ExitState()
    {
        // Tắt animation Block
        _stateData.AnimationHandler.SetBool(AnimationName.Block, false);

        // Tắt trạng thái Block trong Controller
        _blockController?.SetBlocking(false);
    }
}
