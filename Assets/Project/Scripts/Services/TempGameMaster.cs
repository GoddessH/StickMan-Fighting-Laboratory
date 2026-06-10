using System.Collections.Generic;

public class TempGameMaster : Singleton<TempGameMaster>
{
    // 
    private Stack<Character> _playerStack = new Stack<Character>();

    protected override void Awake()
    {
        base.Awake();
    }

    public void AddPlayer(Character character)
    {
        if (_playerStack.Count >= 2) return;

        _playerStack.Push(character);

        if (_playerStack.Count == 2)
        {
            Character playerA = _playerStack.Pop();
            Character playerB = _playerStack.Pop();
            playerA?.GetComponent<RotationHandler>()?.SetTarget(playerB?.transform);
            playerB.GetComponent<RotationHandler>()?.SetTarget(playerA?.transform);
        }
    }
}
