using UnityEngine;

public class TrainingRoomManager : MonoBehaviour
{
    //
    [SerializeField] private Character _characterA;
    [SerializeField] private Character _characterB;

    private RoomManager _gameMaster;

    private void Awake()
    {
        _gameMaster = GetComponent<RoomManager>();
    }

    private void Start()
    {
        if (_gameMaster != null && _characterA != null && _characterB != null)
        {
            _gameMaster.AddPlayer(_characterA);
            _gameMaster.AddPlayer(_characterB);
        }
    }
}
