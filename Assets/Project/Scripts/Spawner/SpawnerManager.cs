using UnityEngine;

public class SpawnerManager : SingletonPun<SpawnerManager>
{
    //
    private CharacterSpawner _characterSpawner;
    private VFXSpawner _vfxSpawner;

    public CharacterSpawner CharacterSpawner => _characterSpawner;
    public VFXSpawner VFXSpawner => _vfxSpawner;


    protected override void Awake()
    {
        base.Awake();

        _characterSpawner = GetComponent<CharacterSpawner>();
        _vfxSpawner = GetComponent<VFXSpawner>();
    }
}
