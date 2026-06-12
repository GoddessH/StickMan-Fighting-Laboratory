using System;
using UnityEngine;

[RequireComponent(typeof(CharacterInput))]
public class BlockController : MonoBehaviour, IProvider<Func<bool>>
{
    //
    [Header("VFX")]
    [SerializeField] private GameObject _blockFlashPrefab;
    [SerializeField] private Transform _blockFlashSpawnPoint;

    private IStateRequestReceiver _requestReceiver;
    private EventInput _blockInput;

    #region Supporter
    private StateRequester _blockRequester;
    #endregion

    public bool IsBlocking { get; private set; }

    private void Awake()
    {
        _requestReceiver = GetComponent<IStateRequestReceiver>();
        _blockRequester = new StateRequester(StateType.Block);

        _blockInput = GetComponent<CharacterInput>().BlockInput;
    }

    private void OnEnable()
        => _blockInput?.SubscribeInputAction(RequestBlock);

    private void OnDisable()
        => _blockInput?.UnsubscribeInputAction();

    private void RequestBlock()
        => _blockRequester?.RequestState(_requestReceiver);

    public void SetBlocking(bool value)
    {
        IsBlocking = value;
    }

    public void SpawnBlockFlashVFX()
    {
        if (_blockFlashPrefab == null) return;
        Transform spawnPoint = _blockFlashSpawnPoint != null ? _blockFlashSpawnPoint : transform;
        GameObject.Instantiate(_blockFlashPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    #region Implement IProvider
    /// <summary>
    /// Provide to BlockState
    /// </summary>
    public Func<bool> Provide()
        => _blockInput?.Provide();
    #endregion
}
