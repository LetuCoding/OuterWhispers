using UnityEngine;

/// <summary>
/// Estado de caída libre del jugador.
/// Permite dashear en el aire, aterrizar y agarrarse a una pared.
/// </summary>
public class FallingState : PlayerState
{
    public FallingState(PlayerStateMachine fsm, Player player) : base(fsm, player) { }

    public override void Enter()
    {
        Player._animator.Play(Player._lastInput >= 0 ? "Fall_Right" : "Fall_Left");
    }

    public override void LogicUpdate()
    {
        // Dash aéreo disponible
        if (Player.dashPressed && Player._canDashAir)
        {
            fsm.ChangeState(Player.DashState);
            return;
        }

        // Aterrizaje
        if (Player._isGrounded)
        {
            Player._audioManager.PlaySFX(Player.footstep, Player.sfxSource, 1f);
            fsm.ChangeState(Player.IdleState);
            return;
        }

        // Contacto con pared
        if (Player._isOnWall)
        {
            fsm.ChangeState(Player.WallSlideState);
        }
    }

    /// <summary>Gravedad aumentada para una caída rápida y responsiva.</summary>
    public override void PhysicsUpdate()
    {
        Player.Gravity(4.5f);
    }

    public override void Exit() { }
}