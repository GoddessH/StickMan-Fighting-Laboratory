using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : SingletonPun<RoomManager>
{
    //
    private NetworkPositionSynchronizer _nps;

    private List<Character> _participantLIst;

    public List<Character> ParticipantList => _participantLIst;
    public NetworkPositionSynchronizer NPS
    {
        get
        {
            _nps = ComponentEnsurer.EnsureComponent(GetComponent<NetworkPositionSynchronizer>(), gameObject);
            return _nps;
        }
    }
}
