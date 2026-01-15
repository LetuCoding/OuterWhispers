using UnityEngine;
using System.Collections;
using Enemies.Interfaces;

public class PatrolBehaviour : MonoBehaviour, IEnemyBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private EnemyStats stats;

    private int currentWaypoint = 0;
    private bool isWaiting = false;
    private Transform enemyTransform;

    public void Enter()
    {
        enemyTransform = transform;
    }

    public void Execute()
    {
        if (isWaiting || waypoints.Length == 0)
            return;

        MoveToWaypoint();
    }

    public void Exit()
    {
    }

    private void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypoint];

        enemyTransform.position = Vector2.MoveTowards(
            enemyTransform.position,
            target.position,
            stats.speed * Time.deltaTime
        );

        if (Vector2.Distance(enemyTransform.position, target.position) < 0.05f)
        {
            StartCoroutine(WaitAndNextWaypoint());
        }
    }

    private IEnumerator WaitAndNextWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(stats.patrolWaitTime);

        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        isWaiting = false;
    }
}