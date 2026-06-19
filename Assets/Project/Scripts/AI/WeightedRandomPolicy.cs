using UnityEngine;

public class WeightedRandomPolicy : IBotDecisionPolicy
{
    public BotBrain.AIState PickAttackAction(BotDifficultyConfig config, PlayerPatternTracker tracker)
    {
        float attack = config.Weights.attackWeight;
        float idle = config.Weights.idleWeight;
        return Random.Range(0f, attack + idle) < attack ? BotBrain.AIState.Attack : BotBrain.AIState.Idle;
    }

    public BotBrain.AIState PickThreatReaction(BotDifficultyConfig config, PlayerPatternTracker tracker)
    {
        float block = config.Weights.blockWeight;
        float retreat = config.Weights.retreatWeight;
        float idle = config.Weights.idleWeight;

        float total = block + retreat + idle;
        float roll = Random.Range(0f, total);

        if (roll < block) return BotBrain.AIState.Block;
        if (roll < block + retreat) return BotBrain.AIState.Retreat;
        return BotBrain.AIState.Idle;
    }
}
