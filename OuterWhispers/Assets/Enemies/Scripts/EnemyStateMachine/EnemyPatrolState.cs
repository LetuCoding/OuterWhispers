using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    private PatrolZone currentZone;
    private int currentWaypointIndex;

    private bool isWaiting;
    private float waitTimer;
    private bool lastDirectionRight;

    // Control de cambio dinámico
    private float zoneCheckTimer;
    private const float zoneCheckInterval = 1f;      // cada 1 segundo
    private const float zoneChangeDistance = 6f;     // distancia mínima para cambiar

    public EnemyPatrolState(EnemyStateMachine stateMachine, Enemy enemy)
        : base(stateMachine, enemy) { }

    public override void Enter()
    {
        isWaiting = false;
        currentWaypointIndex = 0;
        zoneCheckTimer = zoneCheckInterval;

        currentZone = enemy.PatrolZoneService
                           .GetClosestZone(enemy.transform.position);

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
            HandleWaiting();
        else
            MoveToWaypoint();
    }

    private void HandlePlayerDetection()
    {
        if (enemy.playerTransform == null)
            return;

        float dist = Vector2.Distance(
            enemy.transform.position,
            enemy.playerTransform.position);

        if (dist <= enemy.stats.detectionDistance)
        {
            enemy.hasDetectedPlayer = true;
            enemy.audioManager?.StopWalk();

            if (enemy.canShoot)
                stateMachine.ChangeState(enemy.ShootState);
            else if (enemy.canChase)
                stateMachine.ChangeState(enemy.ChaseState);
        }
    }

    private void HandleZoneCheck()
    {
        zoneCheckTimer -= Time.deltaTime;

        if (zoneCheckTimer > 0f)
            return;

        zoneCheckTimer = zoneCheckInterval;

        if (currentZone == null)
            return;

        float sqrDist = (enemy.transform.position - (Vector3)currentZone.Center).sqrMagnitude;

        if (sqrDist > zoneChangeDistance * zoneChangeDistance)
        {
            PatrolZone newZone = enemy.PatrolZoneService
                                       .GetClosestZone(enemy.transform.position);

            if (newZone != null && newZone != currentZone)
            {
                currentZone = newZone;
                currentWaypointIndex = 0;
            }
        }
    }

    private void HandleWaiting()
    {
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
        {
            isWaiting = false;
            currentWaypointIndex =
                (currentWaypointIndex + 1) % currentZone.WaypointCount;
        }
    }

    private void MoveToWaypoint()
    {
        Transform target = currentZone.GetWaypoint(currentWaypointIndex);
        if (target == null)
            return;

        float dirX = target.position.x - enemy.transform.position.x;

        if (Mathf.Abs(dirX) > 0.01f)
        {
            lastDirectionRight = dirX > 0f;

            enemy.animator.Play(lastDirectionRight ? "Walk_Right" : "Walk_Left");
            enemy.audioManager?.PlayWalk();
        }

        enemy.transform.position = Vector2.MoveTowards(
            enemy.transform.position,
            target.position,
            enemy.stats.speed * Time.deltaTime
        );

        if ((enemy.transform.position - target.position).sqrMagnitude < 0.0025f)
            StartWaiting();
    }

    private void StartWaiting()
    {
        enemy.audioManager?.StopWalk();
        enemy.animator.Play(lastDirectionRight ? "Idle_Right" : "Idle_Left");

        isWaiting = true;
        waitTimer = enemy.stats.patrolWaitTime;
    }

    public override void Exit()
    {
        enemy.audioManager?.StopWalk();
    }
}
