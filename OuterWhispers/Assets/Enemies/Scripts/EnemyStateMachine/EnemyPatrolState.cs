using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    private int currentWaypointIndex = 0;
    private bool isWaiting;
    private float waitTimer;
    
    private bool lastDirectionRight; 

    public EnemyPatrolState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        isWaiting = false;
        
        if (enemy.waypoints == null || enemy.waypoints.Length == 0)
        {
            Debug.LogError("No hay waypoints asignados en " + enemy.name);
            stateMachine.ChangeState(new EnemyIdleState(stateMachine, enemy));
        }
    }

    public override void LogicUpdate()
    {
        if (enemy.playerTransform != null)
        {
            float distToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);
            if (distToPlayer <= enemy.stats.detectionDistance)
            {
                enemy.hasDetectedPlayer = true;
                
                if (AudioManagerEnemy.Instance != null) 
                    AudioManagerEnemy.Instance.StopWalk();

                if (enemy.canShoot)
                {
                    stateMachine.ChangeState(enemy.ShootState);
                    return;
                }
                else if (enemy.canChase)
                {
                    stateMachine.ChangeState(enemy.ChaseState);
                    return;
                }
            }
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                isWaiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % enemy.waypoints.Length;
            }
        }
        else
        {
            MoveToWaypoint();
        }
    }

    private void MoveToWaypoint()
    {
        Transform target = enemy.waypoints[currentWaypointIndex];
        float dirX = target.position.x - enemy.transform.position.x;

        if (Mathf.Abs(dirX) > 0.01f) 
        {
            if (dirX > 0f)
            {
                lastDirectionRight = true;
                enemy.animator.Play("Walk_Right");
                
                if (AudioManagerEnemy.Instance != null)
                    AudioManagerEnemy.Instance.PlayWalk();
            }
            else
            {
                lastDirectionRight = false;
                enemy.animator.Play("Walk_Left");

                if (AudioManagerEnemy.Instance != null)
                    AudioManagerEnemy.Instance.PlayWalk();
            }
        }
        
        enemy.transform.position = Vector2.MoveTowards(
            enemy.transform.position,
            target.position,
            enemy.stats.speed * Time.deltaTime
        );
        
        if (Vector2.Distance(enemy.transform.position, target.position) < 0.05f)
        {
            StartWaiting();
        }
    }

    private void StartWaiting()
    {
        if (AudioManagerEnemy.Instance != null)
            AudioManagerEnemy.Instance.StopWalk();

        if (lastDirectionRight == true)
        {
            enemy.animator.Play("Idle_Right");
        } 
        else
        {
            enemy.animator.Play("Idle_Left");
        }
        
        isWaiting = true;
        waitTimer = enemy.stats.patrolWaitTime;
    }
    
    public override void Exit()
    {
        if (AudioManagerEnemy.Instance != null)
            AudioManagerEnemy.Instance.StopWalk();
    }
}