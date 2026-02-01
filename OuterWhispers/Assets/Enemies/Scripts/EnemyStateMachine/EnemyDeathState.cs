using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.StopMovement();

       
        if (enemy.EnemyDirection == true)
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