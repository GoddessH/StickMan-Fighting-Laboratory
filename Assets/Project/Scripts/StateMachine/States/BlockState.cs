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
    private IManaConsumer _manaConsumer;
    private IManaChecker _manaChecker;

    private float _blockTimer;
    private float _manaCost;
    private float _manaTick;

    protected override void SetContext()
    {
        _blockController = _ownerGO.GetComponent<BlockController>();
        _onCheckBlockHolding = _blockController?.Provide();
        _onCheckAttackInput = _ownerGO.GetComponent<MeleeAttackController>()?.Provide();
        _skillController = _ownerGO.GetComponent<SkillController>();
        _manaConsumer = _ownerGO.GetComponent<IManaConsumer>();
        _manaChecker = _ownerGO.GetComponent<IManaChecker>();
    }

    public override void EnterState()
    {
        if (_onCheckBlockHolding == null)
        {
            _onComplete?.Invoke(_type);
            return;
        }

        if (_skillController == null || !_skillController.StartSkill(SkillType.Block))
        {
            _onComplete?.Invoke(_type);
            return;
        }

        var config = _skillController.GetSkillConfig(SkillType.Block);
        _manaCost = config != null ? config.manaCost : 10f;

        // Bật animation Block
        _stateData.AnimationHandler.SetBool(AnimationName.Block, true);
        _blockTimer = 5.0f;
        _manaTick = 0f;
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

        // Tiêu hao mana duy trì mỗi giây
        _manaTick += Time.deltaTime;
        if (_manaTick >= 1.0f)
        {
            if (_manaChecker != null && _manaChecker.HasManaReached(_manaCost))
            {
                _manaConsumer?.ConsumeMana(_manaCost);
                _manaTick -= 1.0f;
            }
            else
            {
                // Hết mana thì thoát Block
                _onComplete?.Invoke(_type);
                return;
            }
        }

        // Cập nhật kỹ năng
        if (_skillController != null)
        {
            if (!_skillController.IsSkillExecuting(SkillType.Block))
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

        // Dừng skill (sẽ tự động gọi OnEnd và giải phóng BlockController)
        _skillController?.StopSkill(SkillType.Block);
    }
}
