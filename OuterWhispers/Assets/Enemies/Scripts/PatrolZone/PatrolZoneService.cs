using UnityEngine;
using System.Collections.Generic;
using Enemies.Scripts;

public class PatrolZoneService : IPatrolZoneService
{
    private readonly List<PatrolZone> _zones = new();

    public void Register(PatrolZone zone) => _zones.Add(zone);

    public void Unregister(PatrolZone zone) => _zones.Remove(zone);

    public PatrolZone GetClosestZone(Vector2 position)
    {
        PatrolZone closest = null;
        float minDist = float.MaxValue;

        foreach (var zone in _zones)
        {
            float dist = Vector2.Distance(position, zone.Center);
            if (dist < minDist)
            {
                minDist = dist;
                closest = zone;
            }
        }
        return closest;
    }
}