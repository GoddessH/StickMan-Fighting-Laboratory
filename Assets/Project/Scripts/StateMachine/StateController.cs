using Photon.Pun;
using System;
using UnityEditor;
using UnityEngine;

public class StateController: MonoBehaviourPun, IStateRequestReceiver
{
    //
    #region Supporter
    [SerializeField] private StateLibrary _stateLibrary = new StateLibrary();

    private StateDataPreparer _stateDataPreparer = new StateDataPreparer();
    private StateMachine _stateMachine = new StateMachine();
    #endregion

    private void Update()
    {
        _stateMachine.Update();
    }

    private void ReceiveCompleteRequest(StateType type)
    {
        if (_stateMachine.CurrentStateType != type) return;

        _stateMachine.SwitchState(StateType.Idle);
    }

    public void Init(GameObject ownerGO)
    {
        _stateDataPreparer.Init(ownerGO);
        _stateLibrary.Init(gameObject, _stateDataPreparer.Provide(), ReceiveCompleteRequest);
        _stateMachine.Init(_stateLibrary);
    }

    #region Explicit Implement IStateRequestReceiver
    void IStateRequestReceiver.ReceiveStateRequest(StateType type)
    {
        if (photonView != null && !photonView.IsMine) return;

        State currentState = _stateLibrary[_stateMachine.CurrentStateType];
        State nextState = _stateLibrary[type];

        if (currentState == null || nextState == null || nextState == currentState || nextState.Priority >= currentState.Priority) return;

        _stateMachine.SwitchState(type);
    }
    #endregion
}
