using UnityEngine;

/// <summary>
/// Estado de deslizamiento en pared. Reduce la gravedad para que el jugador
/// baje lentamente; permite dash, wall-jump y caída libre al separarse.
/// </summary>
public class WallSlideState : PlayerState
{
    public WallSlideState(PlayerStateMachine fsm, Player player) : base(fsm, player) { }

    public override void Enter()
    {
        Player._wallSliding = true;

        Player._audioManager.PlaySFX(Player.slide, Player.sfxSource, 1f);

        Player._animator.Play(Player._isOnRightWall ? "Slide_Right" : "Slide_Left");

        // Restaurar dash aéreo al tocar pared (mechanic estándar de plataformeros)
        Player._canDashAir = true;
    }

    public override void LogicUpdate()
    {
        if (Player.dashPressed)
        {
            fsm.ChangeState(Player.DashState);
            return;
        }

        if (Player.jumpPressed)
        {
            Player.WallJump();
            fsm.ChangeState(Player.JumpState);
            return;
        }

        // El jugador se separó de la pared
        if (!Player._isOnWall)
        {
            fsm.ChangeState(Player.FallingState);
        }
    }

    /// <summary>
    /// Aplica gravedad reducida y fija la velocidad Y al valor de slide
    /// para un descenso suave y controlado.
    /// </summary>
    public override void PhysicsUpdate()
    {
        // FIX (documentación): wallSlideGravity ahora es un campo serializable
        //      (ver Player.cs). El valor "1f" es configurable desde el Inspector.
        Player.Gravity(Player.wallSlideGravity);
        Player._rigidbody2D.linearVelocity = new Vector2(
            Player._rigidbody2D.linearVelocity.x,
            -Player.wallSlideGravity
        );
    }

    public override void Exit()
    {
        Player._wallSliding = false;
    }
}