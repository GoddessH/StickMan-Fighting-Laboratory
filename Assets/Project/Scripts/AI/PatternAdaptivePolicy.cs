using UnityEngine;

public class PatternAdaptivePolicy : IBotDecisionPolicy
{
    public BotBrain.AIState PickAttackAction(BotDifficultyConfig config, PlayerPatternTracker tracker)
    {
        float attack = config.attackWeight;
        float idle = config.idleWeight;

        if (config.enablePatternTracking && tracker != null)
        {
            float defScale = tracker.DefensivenessScore * config.patternAdaptationStrength;
            attack += config.attackWeight * defScale;

            if (defScale > 0.3f)
            {
                Debug.Log($"[BotBrain] Thích ứng: Người chơi thủ nhiều (Defensiveness: {tracker.DefensivenessScore:F2}), tăng attackWeight -> {attack:F1}");
            }
        }

        return Random.Range(0f, attack + idle) < attack ? BotBrain.AIState.Attack : BotBrain.AIState.Idle;
    }

    public BotBrain.AIState PickThreatReaction(BotDifficultyConfig config, PlayerPatternTracker tracker)
    {
        float block = config.blockWeight;
        float retreat = config.retreatWeight;
        float idle = config.idleWeight;

        if (config.enablePatternTracking && tracker != null)
        {
            float aggressionScale = tracker.AggressionScore * config.patternAdaptationStrength;
            block += config.blockWeight * aggressionScale;
            retreat += config.retreatWeight * aggressionScale;
            idle = Mathf.Max(0f, idle - idle * aggressionScale * 0.5f);

            Debug.Log($"[BotBrain] Thích ứng: Người chơi tấn công nhiều (Aggression: {tracker.AggressionScore:F2}), tăng blockWeight -> {block:F1}, retreatWeight -> {retreat:F1}");
        }

        float total = block + retreat + idle;
        float roll = Random.Range(0f, total);

        if (roll < block) return BotBrain.AIState.Block;
        if (roll < block + retreat) return BotBrain.AIState.Retreat;
        return BotBrain.AIState.Idle;
    }
}
