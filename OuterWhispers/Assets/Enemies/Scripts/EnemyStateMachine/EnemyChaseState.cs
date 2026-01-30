using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void LogicUpdate()
    {
        if (enemy.playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);

        if (distanceToPlayer <= enemy.stats.attackRange.x)
        {
            enemy.DecideNextCombatAction();
            return;
        }

        if (distanceToPlayer > enemy.stats.detectionDistance * 1.5f)
        {
            if (enemy.EnemyDirection)
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
        
        Vector3 targetPosition = new Vector3(enemy.playerTransform.position.x, enemy.transform.position.y, enemy.transform.position.z);

        enemy.transform.position = Vector3.MoveTowards(
            enemy.transform.position,
            targetPosition,
            enemy.stats.speed * Time.deltaTime
        );
        
        
        bool isPlayerRight = enemy.playerTransform.position.x > enemy.transform.position.x;

        if (isPlayerRight)
        {
            enemy.animator.Play("Walk_Right");
            
            if (AudioManagerEnemy.Instance != null) 
                AudioManagerEnemy.Instance.PlayWalk();
        }
        else
        {
            enemy.animator.Play("Walk_Left");
            
            if (AudioManagerEnemy.Instance != null) 
                AudioManagerEnemy.Instance.PlayWalk();
        }
        
        if (enemy.meleeHitbox != null)
        {
            float direction = isPlayerRight ? 1f : -1f;
            enemy.meleeHitbox.transform.localPosition = new Vector3(direction * enemy.meleeAttackOffset, 0, 0);
        }
    }

    public override void Exit()
    {
        if (AudioManagerEnemy.Instance != null)
            AudioManagerEnemy.Instance.StopWalk();
    }
}