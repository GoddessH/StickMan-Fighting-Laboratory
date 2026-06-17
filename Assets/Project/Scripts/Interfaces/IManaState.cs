using UnityEngine;

public interface IManaState
{
    public float GetCurrentMana();
    public bool HasEnoughMana(float amount);
}
