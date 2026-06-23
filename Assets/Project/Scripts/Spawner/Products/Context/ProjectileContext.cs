using System;
using UnityEngine;

public class ProjectileContext
{
    // 
    public Vector2 SpawnPosition { get; set; }
    public Vector2 Direction { get; set; }
    public Action<HurtPoint> OnDoDamages { get; set; }

    public ProjectileContext() { }
    public ProjectileContext(ProjectileContext other)
    {
        SpawnPosition = other.SpawnPosition;
        Direction = other.Direction;
        OnDoDamages = other.OnDoDamages;
    }
}
