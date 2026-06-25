using UnityEngine;

public class StateDataPreparer : IProvider<StateData>
{
    // 
    private GameObject _ownerGO;
    public void Init(GameObject ownerGO)
        => _ownerGO = ownerGO;

    #region Implement IProvider
    public StateData Provide()
    {
        AnimationHandle animationHandler = ComponentEnsurer.EnsureComponent(_ownerGO.GetComponent<AnimationHandle>(), _ownerGO);

        StateData preparedContext = new StateData(animationHandler);

        return preparedContext;
    }
    #endregion
}
