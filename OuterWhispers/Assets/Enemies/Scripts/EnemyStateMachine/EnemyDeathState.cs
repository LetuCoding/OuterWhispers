using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.StopMovement();

        // 2. Reproducir animación de muerte
        // ¡Asegúrate de crear esta animación en tu Animator!
        //enemy.animator.Play("Death");

        enemy.DisablePhysics();
    }

    public override void LogicUpdate()
    {
    }

    public override void PhysicsUpdate()
    {
    }
}