using UnityEngine;

/// <summary>
/// Estado base del jugador cuando está en el suelo sin acción especial.
/// Gestiona las transiciones hacia Sprint, Jump, Dash, Attack y Fall.
/// </summary>
public class IdleState : PlayerState
{
    public IdleState(PlayerStateMachine fsm, Player player) : base(fsm, player) { }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");

        // Reproducir animación según la última dirección registrada
        Player._animator.Play(Player._lastInput < 0 ? "Idle_Left" : "Idle_Right");
        Player._jumpCutting = false;
    }

    public override void LogicUpdate()
    {
        // ── Sprint ──────────────────────────────────────────────────────────
        // Shift pulsado este frame: dash lateral instantáneo desde parado
        if (Player._isGrounded && Player.shiftPressedThisFrame)
        {
            Player._audioManager.StopWalk(Player.sfxSource);
            fsm.ChangeState(Player.DashState);
            return;
        }

        // Shift mantenido + movimiento: transición a sprint
        if (Player._moveInput != 0f && Player.shiftHeld)
        {
            Player.footsetpPitch = 0.7f;
            Player._audioManager.PlayWalk(Player.footstep, Player.sfxSource, Player.footsetpPitch);
            fsm.ChangeState(Player.SprintState);
            return;
        }

        // ── Dash ─────────────────────────────────────────────────────────────
        if (Player.dashPressed)
        {
            Player._audioManager.StopWalk(Player.sfxSource);
            fsm.ChangeState(Player.DashState);
            return;
        }

        // ── Jump ─────────────────────────────────────────────────────────────
        if (Player.jumpPressed && Player._isGrounded)
        {
            Player._audioManager.StopWalk(Player.sfxSource);
            fsm.ChangeState(Player.JumpState);
            return;
        }

        // ── Attack ───────────────────────────────────────────────────────────
        // FIX: usamos attackPressed (nuevo Input System) en lugar de
        //      Input.GetKeyDown(KeyCode.F) (API clásica, inconsistente).
        // FIX: movido ANTES de los bloques de walk para que no quede enmascarado.
        if (Player.attackPressed)
        {
            fsm.ChangeState(Player.AttackState);
            return;
        }

        // ── Walk animations ──────────────────────────────────────────────────
        if (Player._moveInput > 0f)
        {
            Player.footsetpPitch = 0.5f;
            Player._animator.Play("Walk_Right");
            Player._audioManager.PlayWalk(Player.footstep, Player.sfxSource, Player.footsetpPitch);
        }
        else if (Player._moveInput < 0f)
        {
            Player.footsetpPitch = 0.5f;
            Player._animator.Play("Walk_Left");
            Player._audioManager.PlayWalk(Player.footstep, Player.sfxSource, Player.footsetpPitch);
        }
        else
        {
            // Sin input: volver a idle con la animación correcta
            Player._audioManager.StopWalk(Player.sfxSource);
            Player._animator.Play(Player._lastInput < 0 ? "Idle_Left" : "Idle_Right");
        }

        // ── Caída ────────────────────────────────────────────────────────────
        if (!Player._isGrounded && Player._rigidbody2D.linearVelocity.y < 0f)
        {
            fsm.ChangeState(Player.FallingState);
        }
    }

    /// <summary>Gravedad estándar del jugador en el suelo.</summary>
    public override void PhysicsUpdate()
    {
        Player.Gravity(3.5f);
    }

    public override void Exit()
    {
        Player._audioManager.StopWalk(Player.sfxSource);
    }
}