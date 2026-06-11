using System.Collections.Generic;
using UnityEngine;

public class UIInMatchManager : MonoBehaviour
{
    //
    [SerializeField] private UIInMatchTop _topUI;


    public void ConnectToFighter(Stack<Character> fighterStack)
    {
        _topUI?.SetupFighter(fighterStack);
    }
}
