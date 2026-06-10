using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : SingletonPun<RoomManager>
{
    //
    private List<Character> _participantLIst;

    public List<Character> ParticipantList => _participantLIst;
}
