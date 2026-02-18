using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    private PatrolZone currentZone;
    private int currentWaypointIndex;

    private bool isWaiting;
    private float waitTimer;
    private bool lastDirectionRight;

    private float zoneCheckTimer;
    private const float zoneCheckInterval = 1f;
    private const float zoneChangeDistance = 6f;
    private const float stopDistance = 0.2f; 

    public EnemyPatrolState(EnemyStateMachine stateMachine, Enemy enemy)
        : base(stateMachine, enemy) { }

    public override void Enter()
    {
        isWaiting = false;
        currentWaypointIndex = 0;
        zoneCheckTimer = zoneCheckInterval;

        currentZone = enemy.PatrolZoneService.GetClosestZone(enemy.transform.position);

        if (currentZone == null || currentZone.WaypointCount < 2)
        {
            Debug.LogError("No hay PatrolZone válida para " + enemy.name);
            stateMachine.ChangeState(enemy.IdleState);
            return;
        }
    }

    public override void LogicUpdate()
    {
        HandlePlayerDetection();
        HandleZoneCheck();

        if (isWaiting)
        {
            HandleWaiting();
        }
        else
        {
            MoveToWaypoint();
        }
    }

    private void HandlePlayerDetection()
    {
        if (enemy.playerTransform == null) return;

        float dist = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);

        if (dist <= enemy.stats.detectionDistance)
        {
            enemy.hasDetectedPlayer = true;
            enemy.rb.linearVelocity = new Vector2(0, enemy.rb.linearVelocity.y); 
            enemy._audioManager.StopWalk(enemy.audioSource);

            if (enemy.canShoot)
                stateMachine.ChangeState(enemy.ShootState);
            else if (enemy.canChase)
                stateMachine.ChangeState(enemy.ChaseState);
        }
    }

    private void HandleZoneCheck()
    {
        zoneCheckTimer -= Time.deltaTime;
        if (zoneCheckTimer > 0f) return;

        zoneCheckTimer = zoneCheckInterval;

        if (currentZone == null) return;

        float distX = Mathf.Abs(enemy.transform.position.x - currentZone.Center.x);
        
        if (distX > zoneChangeDistance)
        {
            PatrolZone newZone = enemy.PatrolZoneService.GetClosestZone(enemy.transform.position);
            if (newZone != null && newZone != currentZone)
            {
                currentZone = newZone;
                currentWaypointIndex = 0;
            }
        }
    }

    private void HandleWaiting()
    {
        enemy.rb.linearVelocity = new Vector2(0, enemy.rb.linearVelocity.y);

        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
        {
            isWaiting = false;
            currentWaypointIndex = (currentWaypointIndex + 1) % currentZone.WaypointCount;
        }
    }

    private void MoveToWaypoint()
    {
        Transform target = currentZone.GetWaypoint(currentWaypointIndex);
        if (target == null) return;

        float distanceX = Mathf.Abs(target.position.x - enemy.transform.position.x);

        if (distanceX < stopDistance)
        {
            StartWaiting();
            return;
        }

        float direction = Mathf.Sign(target.position.x - enemy.transform.position.x);

        if (distanceX > 0.01f)
        {
            lastDirectionRight = direction > 0f;
            enemy.animator.Play(lastDirectionRight ? "Walk_Right" : "Walk_Left");
            enemy._audioManager.PlayWalk(enemy.footstep, enemy.audioSource, enemy.pitch);
        }
        
        enemy.rb.linearVelocity = new Vector2(direction * enemy.stats.speed, enemy.rb.linearVelocity.y);
    }

    private void StartWaiting()
    {
        enemy.rb.linearVelocity = new Vector2(0, enemy.rb.linearVelocity.y);
        
        enemy._audioManager.StopWalk(enemy.audioSource);
        enemy.animator.Play(lastDirectionRight ? "Idle_Right" : "Idle_Left");

        isWaiting = true;
        waitTimer = enemy.stats.patrolWaitTime;
    }

    public override void Exit()
    {
        if (enemy.rb != null)
        {
            enemy.rb.linearVelocity = new Vector2(0, enemy.rb.linearVelocity.y);
        }
        
        enemy._audioManager.StopWalk(enemy.audioSource);
    }
}