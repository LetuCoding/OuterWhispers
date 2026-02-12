using UnityEngine;
namespace Enemies.Scripts
{
    public interface IPatrolZoneService
    {
        PatrolZone GetClosestZone(Vector2 position);

        public void Register(PatrolZone zone);
        public void Unregister(PatrolZone zone);
        
    }
}