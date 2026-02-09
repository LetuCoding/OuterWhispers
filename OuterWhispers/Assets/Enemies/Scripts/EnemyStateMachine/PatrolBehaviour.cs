using UnityEngine;
using System.Collections;
using Enemies.Interfaces;

public class PatrolBehaviour : MonoBehaviour, IEnemyBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private EnemyStats stats;
    public Animator _animator;

    private int currentWaypoint = 0;
    private bool isWaiting = false;
    private bool direction = false;
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
        float dirX = target.position.x - enemyTransform.position.x;

        if (dirX > 0f)
        {
            direction = true;
            _animator.Play("Walk_Right");
            //AudioManagerEnemy.Instance.PlayWalk();
        }
        else if (dirX < 0f)
        {
            direction = false;
            _animator.Play("Walk_Left");
            //AudioManagerEnemy.Instance.PlayWalk();
        }
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
        //AudioManagerEnemy.Instance.StopWalk();
        if (direction == true)
        {
            _animator.Play("Idle_Right");
        } else
        {
            _animator.Play("Idle_Left");
        }
        isWaiting = true;
        yield return new WaitForSeconds(stats.patrolWaitTime);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        isWaiting = false;
    }
}