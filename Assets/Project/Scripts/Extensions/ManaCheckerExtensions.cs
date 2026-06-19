using UnityEngine;

public static class ManaCheckerExtensions
{
    public static float GetManaPercent(this IManaChecker mana)
    {
        if (mana == null) return 100f;
        float max = mana.GetMaxMana();
        return max > 0 ? (mana.GetCurrentMana() / max) * 100f : 100f;
    }
}
