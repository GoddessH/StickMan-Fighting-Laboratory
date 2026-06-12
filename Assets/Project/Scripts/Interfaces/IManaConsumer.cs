using UnityEngine;

public interface IManaConsumer
{
    //
    public float CurrentMana { get; }
    public bool HasEnoughMana(float amount);
    public void ConsumeMana(float amount);
}
