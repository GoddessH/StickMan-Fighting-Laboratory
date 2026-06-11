using System;
using UnityEngine;

public interface IDamageTakerEvent
{
    //
    public void SubscribeTakeDamageEvent(Action subscriber);
    public void UnsubscribeTakeDamageEvent(Action unSubscriber);
}
