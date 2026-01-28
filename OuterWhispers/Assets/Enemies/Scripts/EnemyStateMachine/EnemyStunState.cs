using UnityEngine;

public class EnemyStunState : EnemyState
{
    private float stunTimer;

    public EnemyStunState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        // 1. Detener movimiento inmediatamente
        enemy.StopMovement();

        // 2. Reproducir animación de herido
        // Asegúrate de tener una animación llamada "Hurt" o cambia este nombre
        enemy.animator.Play("Hurt"); 

        // 3. Reiniciar temporizador
        stunTimer = enemy.stunDuration;
    }

    public override void LogicUpdate()
    {
        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0)
        {
            // El stun ha terminado, decidimos qué hacer ahora.
            
            // Si el jugador sigue vivo y detectado...
            if (enemy.playerTransform != null)
            {
                float distance = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);

                // Si está muy cerca, atacar
                if (distance <= enemy.stats.attackRange.x)
                {
                    stateMachine.ChangeState(enemy.MeleeState);
                }
                // Si puede disparar y está en rango, disparar
                else if (enemy.canShoot && distance <= enemy.stats.detectionDistance)
                {
                    stateMachine.ChangeState(enemy.ShootState);
                }
                // Si no, perseguir
                else
                {
                    stateMachine.ChangeState(enemy.ChaseState);
                }
            }
            else
            {
                // Si perdió al jugador, volver a patrulla
                stateMachine.ChangeState(enemy.PatrolState);
            }
        }
    }

    public override void Exit()
    {
        // Opcional: Resetear algún color si lo cambiaste a rojo al recibir daño
    }
}