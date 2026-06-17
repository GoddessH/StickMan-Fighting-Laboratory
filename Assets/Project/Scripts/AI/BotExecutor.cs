using UnityEngine;

[RequireComponent(typeof(BotInput))]
public class BotExecutor : MonoBehaviour, IBotExecutor
{
    private BotInput _botInput;

    public void Init()
    {
        _botInput = GetComponent<BotInput>();
    }

    public void ClearAttackInput()
    {
        if (_botInput != null && _botInput.BotAttack != null)
        {
            _botInput.BotAttack.Clear();
        }
    }

    public void SetMovement(Vector2 direction)
    {
        if (_botInput != null && _botInput.BotMovement != null)
        {
            _botInput.BotMovement.SetDirection(direction);
        }
    }

    public void TriggerAttack()
    {
        if (_botInput != null && _botInput.BotAttack != null)
        {
            _botInput.BotAttack.TriggerAttack();
        }
    }

    public void StartBlock()
    {
        if (_botInput != null && _botInput.BotBlock != null)
        {
            _botInput.BotBlock.StartBlock();
        }
    }

    public void StopBlock()
    {
        if (_botInput != null && _botInput.BotBlock != null)
        {
            _botInput.BotBlock.StopBlock();
        }
    }
}
