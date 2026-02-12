using System;
using UnityEngine;
using Zenject;
using Enemies.Scripts;

    public class PatrolZone : MonoBehaviour
    {
        [Inject] IPatrolZoneService _zoneService;

        private void OnEnable() => _zoneService.Register(this);

        private void OnDisable() => _zoneService.Unregister(this);
        
        
        
        public Transform[] waypoints;

        public Vector2 Center
        {
            get
            {
                Vector2 sum = Vector2.zero;
                foreach (var wp in waypoints)
                    sum += (Vector2)wp.position;
                return sum / waypoints.Length;
            }
        }

        public Transform GetWaypoint(int index)
        {
            if (waypoints == null || waypoints.Length == 0)
                return null;

            return waypoints[index % waypoints.Length];
        }

        public int WaypointCount => waypoints.Length;

        public void OnDrawGizmos()
        {
            foreach (var wp in waypoints)
            {
                Gizmos.DrawWireSphere(wp.position, 0.2f);
            }
        }
    }


