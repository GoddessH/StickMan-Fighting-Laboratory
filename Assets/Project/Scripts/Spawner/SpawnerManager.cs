using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : SingletonPun<SpawnerManager>
{
    //
    private ProjectileSpawner _projectileSpawner;
    private CharacterSpawner _characterSpawner;
    private VFXSpawner _vfxSpawner;

    public CharacterSpawner CharacterSpawner => _characterSpawner;
    public VFXSpawner VFXSpawner => _vfxSpawner;
    public ProjectileSpawner ProjectileSpawner => _projectileSpawner;


    protected override void Awake()
    {
        base.Awake();

        _characterSpawner = GetComponent<CharacterSpawner>();
        _vfxSpawner = GetComponent<VFXSpawner>();
        _projectileSpawner = GetComponent<ProjectileSpawner>();
    }
}
