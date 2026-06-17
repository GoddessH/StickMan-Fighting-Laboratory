using UnityEngine;

public interface IManaChecker
{
    // 
    public bool IsFullMana();

    /// <summary>
    /// Checks whether current maan has reached the given amount
    /// </summary>
    /// <returns></returns>
    public bool HasManaReached(float amount);
    public float GetCurrentMana();
}
