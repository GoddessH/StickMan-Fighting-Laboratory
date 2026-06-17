using UnityEngine;

public class BlockSkill : Skill
{
    private BlockController _blockController;

    public override void Init(GameObject owner, BaseSkill config)
    {
        base.Init(owner, config);
        _blockController = owner.GetComponent<BlockController>();
    }

    protected override void ExecuteLogic()
    {
        _blockController?.SetBlocking(true);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        _blockController?.SetBlocking(false);
    }
}
