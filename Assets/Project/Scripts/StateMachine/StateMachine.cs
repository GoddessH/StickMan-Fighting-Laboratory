using UnityEngine;

public class StateMachine
{
    //
    private StateLibrary _library;
    public StateType CurrentStateType { get; private set; }

    public void Init(StateLibrary library)
    {
        _library = library;
        CurrentStateType = StateType.Idle;
        _library[CurrentStateType].EnterState();
    }

    public void Update()
    {
        if (_library == null) return;
        _library[CurrentStateType].UpdateState();
    }
    
    public void SwitchState(StateType nextType)
    {
        //Debug.Log($"{CurrentStateType} ---> {nextType}");
        _library[CurrentStateType].ExitState();
        CurrentStateType = nextType;
        _library[CurrentStateType].EnterState();
    }
}
