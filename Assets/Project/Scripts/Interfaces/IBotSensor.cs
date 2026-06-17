using UnityEngine;

public interface IBotSensor
{
    Transform Target { get; }
    float Distance { get; }
    float DistanceY { get; }
    Vector2 DirectionToTarget { get; }
    bool ThreatDetected { get; }

    void Init(BotDifficultyConfig config, LayerMask playerHitBoxMask);
    void SetTarget(Transform target);
    void UpdateSensor();
}
