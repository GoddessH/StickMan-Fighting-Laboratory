using UnityEngine;

public class PatternAdaptivePolicy : IBotDecisionPolicy
{
    public BotBrain.AIState PickAttackAction(BotDifficultyConfig config, PlayerPatternTracker tracker)
    {
        float attack = config.Weights.attackWeight;
        float idle = config.Weights.idleWeight;

        if (config.Pattern.enablePatternTracking && tracker != null)
        {
            float defScale = tracker.DefensivenessScore * config.Pattern.patternAdaptationStrength;
            attack += config.Weights.attackWeight * defScale;
        }

        return Random.Range(0f, attack + idle) < attack ? BotBrain.AIState.Attack : BotBrain.AIState.Idle;
    }

    public BotBrain.AIState PickThreatReaction(BotDifficultyConfig config, PlayerPatternTracker tracker)
    {
        float block = config.Weights.blockWeight;
        float retreat = config.Weights.retreatWeight;
        float idle = config.Weights.idleWeight;

        if (config.Pattern.enablePatternTracking && tracker != null)
        {
            float aggressionScale = tracker.AggressionScore * config.Pattern.patternAdaptationStrength;
            block += config.Weights.blockWeight * aggressionScale;
            retreat += config.Weights.retreatWeight * aggressionScale;
            idle = Mathf.Max(0f, idle - idle * aggressionScale * 0.5f);
        }

        float total = block + retreat + idle;
        float roll = Random.Range(0f, total);

        if (roll < block) return BotBrain.AIState.Block;
        if (roll < block + retreat) return BotBrain.AIState.Retreat;
        return BotBrain.AIState.Idle;
    }
}
