using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.StopMovement();
        bool isPlayerRight = enemy.playerTransform.position.x > enemy.transform.position.x;
        enemy.EnemyDirection = isPlayerRight;
        if (AudioManagerEnemy.Instance != null)
            AudioManagerEnemy.Instance.PlaySFX(AudioManagerEnemy.Instance.dead);
        if (isPlayerRight)
        {
            enemy.animator.Play("Death_Right"); 
        }
        else
        {
            enemy.animator.Play("Death_Left"); 
        }

        enemy.DisablePhysics();
    }

    public override void LogicUpdate()
    {
    }

    public override void PhysicsUpdate()
    {
    }
}