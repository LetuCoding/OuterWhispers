using UnityEngine;
using System.Collections;
using Enemies.Interfaces;
using Zenject;

public class PatrolBehaviour : MonoBehaviour, IEnemyBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private EnemyStats stats;
    public Animator _animator;

    private int currentWaypoint = 0;
    private bool isWaiting = false;
    private bool direction = false;
    private Transform enemyTransform;
    private Enemy _enemy;

    [Inject]
    public void Construct(Enemy enemy)
    {
        this._enemy = enemy;
    }
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
        _enemy._audioManager.PlayWalk(_enemy.footstep,_enemy.audioSource,_enemy.pitch);
        if (dirX > 0f)
        {
            direction = true;
            _animator.Play("Walk_Right");
        }
        else if (dirX < 0f)
        {
            direction = false;
            _animator.Play("Walk_Left");
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
        _enemy._audioManager.StopWalk(_enemy.audioSource);
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