using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateLibrary
{
    //
    [SerializeField] private List<StateEntry> _stateEntryList = new List<StateEntry>();

    private Dictionary<StateType, State> _stateDictionary = new Dictionary<StateType, State>();

    public void Init(GameObject ownerGO, StateData stateData, Action<StateType> onComplete)
    {
        foreach(var entry in _stateEntryList)
        {
            State state = entry.StateSO.ProvideState();

            if (state == null) continue;
            StateType type = entry.StateSO.Type;

            state.Init(new InitialStateData(ownerGO, onComplete, stateData, type, entry.Priority));
            _stateDictionary[type] = state;
        }
    }

    public State this[StateType type]
    {
        get
        {
            if (!_stateDictionary.ContainsKey(type)) return null;
            return _stateDictionary[type];
        }
    }
}
