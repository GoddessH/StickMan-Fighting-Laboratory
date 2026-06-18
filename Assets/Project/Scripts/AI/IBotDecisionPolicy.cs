public interface IBotDecisionPolicy
{
    BotBrain.AIState PickAttackAction(BotDifficultyConfig config, PlayerPatternTracker tracker);
    BotBrain.AIState PickThreatReaction(BotDifficultyConfig config, PlayerPatternTracker tracker);
}
