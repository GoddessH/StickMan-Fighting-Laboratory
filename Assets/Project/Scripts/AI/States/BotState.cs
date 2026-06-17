public abstract class BotState
{
    protected BotBrain brain;
    protected IBotSensor sensor;
    protected IBotExecutor executor;
    protected BotDifficultyConfig config => brain.Config;

    protected BotState(BotBrain brain, IBotSensor sensor, IBotExecutor executor)
    {
        this.brain = brain;
        this.sensor = sensor;
        this.executor = executor;
    }

    public virtual void Enter() {}
    public virtual void Update() {}
    public virtual void Exit() {}
}
