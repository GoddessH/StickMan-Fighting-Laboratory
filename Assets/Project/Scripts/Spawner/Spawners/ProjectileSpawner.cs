using UnityEngine;

public class ProjectileSpawner : Spawner<Projectile>
{
    // 
    public void Spawn(ProjectileContext context, int poolID = 0)
    {
        if (poolID < 0 || poolID >= _pools.Length) return;

        Projectile projectile = _pools[poolID].GetRandom();
        projectile.SetContext(context);
    }
}
