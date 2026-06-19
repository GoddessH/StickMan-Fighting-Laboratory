public abstract class BotState
{
    protected IBotContext context;

    public virtual int Priority => 0;

    protected BotState(IBotContext context)
    {
        this.context = context;
    }

    public virtual void Enter() {}
    public virtual void Update() {}
    public virtual void Exit() {}
    public virtual bool CanInterrupt(BotBrain.AIState incomingState) => true;
}
