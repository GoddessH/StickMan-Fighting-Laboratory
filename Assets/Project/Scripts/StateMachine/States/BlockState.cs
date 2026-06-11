using System;

public class BlockState : State
{
    // Delegate từ BlockController — kiểm tra "đang giữ nút Block không?"
    private Func<bool> _onCheckBlockHolding;
    // Delegate từ AttackController — kiểm tra "vừa bấm Attack không?"
    private Func<bool> _onCheckAttackInput;

    private BlockController _blockController;

    protected override void SetContext()
    {
        _blockController = _ownerGO.GetComponent<BlockController>();
        _onCheckBlockHolding = _blockController?.Provide();
        _onCheckAttackInput = _ownerGO.GetComponent<AttackController>()?.Provide();
    }

    public override void EnterState()
    {
        if (_onCheckBlockHolding == null)
        {
            _onComplete?.Invoke(_type);
            return;
        }

        // Bật animation Block
        _stateData.AnimationHandler.SetBool(AnimationName.Block, true);

        // Bật trạng thái Block trong Controller
        _blockController?.SetBlocking(true);
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
