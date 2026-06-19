using UnityEngine;

public interface IBotExecutor
{
    void Init();
    void ClearAttackInput();
    void SetMovement(Vector2 direction);
    void TriggerAttack();
    void StartBlock();
    void StopBlock();
    void StartCharge();
    void StopCharge();
    void TriggerFlash();
}
