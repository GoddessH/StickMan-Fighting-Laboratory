using System.Collections.Generic;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    // 
    private Stack<Character> _fighterStack = new Stack<Character>();

    #region Supporter
    [SerializeField] private RoomInitializer _roomInitializer = new RoomInitializer();
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    public void AddPlayer(Character character)
    {
        if (_fighterStack.Count >= 2) return;

        _fighterStack.Push(character);
        _roomInitializer.InitFighterFacing(new Stack<Character>(_fighterStack));
        _roomInitializer.ConnectToFighter(character);
    }
}
