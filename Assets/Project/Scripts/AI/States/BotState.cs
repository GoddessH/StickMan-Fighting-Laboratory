public abstract class BotState
{
    protected BotBrain brain;
    protected BotSensor sensor;
    protected BotExecutor executor;
    protected BotDifficultyConfig config => brain.Config;

    protected BotState(BotBrain brain, BotSensor sensor, BotExecutor executor)
    {
        this.brain = brain;
        this.sensor = sensor;
        this.executor = executor;
    }

    public virtual void Enter() {}
    public virtual void Update() {}
    public virtual void Exit() {}
}
