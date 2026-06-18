using System;
using UnityEngine;

public interface IDamageDealerEvent
{
    //
    public void SubscribeEvent(Action subscriber);
    public void UnsubscribeEvent(Action subscriber);
}
