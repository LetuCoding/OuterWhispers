using UnityEngine;

public class EnemyStunState : EnemyState
{
    private float stunTimer;

    public EnemyStunState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.StopMovement();
        /*if (enemy.audioManager != null)
            enemy.audioManager.PlaySFX(enemy.audioManager.damage);*/
        if (enemy.EnemyDirection == true)
        {
            enemy.animator.Play("Damage_Right"); 
        }
        else
        {
            enemy.animator.Play("Damage_Left"); 
        }
        stunTimer = enemy.stunDuration;
    }

    public override void LogicUpdate()
    {
        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0)
        {
            if (enemy.playerTransform != null)
            {
                float distance = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);
                if (distance <= enemy.stats.attackRange.x)
                {
                    stateMachine.ChangeState(enemy.MeleeState);
                }
                else if (enemy.canShoot && distance <= enemy.stats.detectionDistance)
                {
                    stateMachine.ChangeState(enemy.ShootState);
                }
                else
                {
                    stateMachine.ChangeState(enemy.ChaseState);
                }
            }
            else
            {
                stateMachine.ChangeState(enemy.PatrolState);
            }
        }
    }

    public override void Exit()
    {
    }
}