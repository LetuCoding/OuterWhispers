using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void LogicUpdate()
    {
        if (enemy.playerTransform == null)
        {
            stateMachine.ChangeState(enemy.PatrolState);
            return;
        }

        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);

        if (distanceToPlayer <= enemy.stats.attackRange.x)
        {
            enemy.rb.linearVelocity = new Vector2(0, enemy.rb.linearVelocity.y);
            enemy.DecideNextCombatAction();
            return;
        }

        if (distanceToPlayer > enemy.stats.detectionDistance * 1.5f)
        {
            if (enemy.transform.position.x < enemy.playerTransform.position.x)
            {
                enemy.animator.Play("Stop_Attack_Right");
            }
            else
            {
                enemy.animator.Play("Stop_Attack_Left");
            }
            
            enemy.hasDetectedPlayer = false;
            stateMachine.ChangeState(enemy.PatrolState);
            return;
        }
        
        float directionX = Mathf.Sign(enemy.playerTransform.position.x - enemy.transform.position.x);

        enemy.rb.linearVelocity = new Vector2(directionX * enemy.stats.speed, enemy.rb.linearVelocity.y);
        
        bool isPlayerRight = directionX > 0;
        enemy._audioManager.PlayWalk(enemy.footstep, enemy.audioSource, enemy.pitch);

        if (isPlayerRight)
        {
            enemy.animator.Play("Walk_Right");
        }
        else
        {
            enemy.animator.Play("Walk_Left");
        }
        
        if (enemy.meleeHitbox != null)
        {
            float hitBoxDirection = isPlayerRight ? 1f : -1f;
            enemy.meleeHitbox.transform.localPosition = new Vector3(hitBoxDirection * enemy.meleeAttackOffset, 0, 0);
        }
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