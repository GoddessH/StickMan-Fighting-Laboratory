public class StateRequester
{
    //
    private StateType _type;
    public StateRequester(StateType type)
        => _type = type;

    public void RequestState(IStateRequestReceiver requestReceiver)
    {
        if (requestReceiver == null) return;

        requestReceiver.ReceiveStateRequest(_type);
    }
}
