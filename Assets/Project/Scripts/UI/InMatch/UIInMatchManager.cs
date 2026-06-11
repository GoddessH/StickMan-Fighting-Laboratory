using System.Collections.Generic;
using UnityEngine;

public class UIInMatchManager : MonoBehaviour
{
    //
    [SerializeField] private UIInMatchTop _topUI;


    public void ConnectToFighter(Character fighter)
    {
        _topUI?.ConnectFighterToValidPanel(fighter);
    }
}
