using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Gameplay.PlayerScripts.STATE_MACHINE
{
    /// <summary>
    /// Estado de carrera del jugador. Duplica la velocidad mientras Shift esté pulsado.
    /// Permite saltar, atacar en carrera, dashear y caer desde este estado.
    /// </summary>
    public class SprintState : PlayerState
    {
        private float _originalSpeed;
        private const float SprintMultiplier = 2f;

        public SprintState(PlayerStateMachine fsm, Player player) : base(fsm, player) { }

        public override void Enter()
        {
            Player._isSprinting = true;
            Debug.Log("Entering Sprint State");

            _originalSpeed = Player.speed;
            Player.speed   = _originalSpeed * SprintMultiplier;

            Player._audioManager.PlayWalk(Player.footstep, Player.sfxSource, Player.footsetpPitch);
        }

        public override void LogicUpdate()
        {
            bool shiftHeld =
                Player._playerInputActions.Player.Dash.IsPressed() ||
                (Keyboard.current.leftShiftKey  != null && Keyboard.current.leftShiftKey.isPressed) ||
                (Keyboard.current.rightShiftKey != null && Keyboard.current.rightShiftKey.isPressed);

            // ── Salir del sprint si se suelta Shift ──────────────────────────
            if (!shiftHeld)
            {
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.IdleState);
                return;
            }

            // ── Sin movimiento horizontal → volver a Idle ────────────────────
            if (Mathf.Approximately(Player._moveInput, 0f))
            {
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.IdleState);
                return;
            }

            // ── Animaciones de sprint ────────────────────────────────────────
            if (Player._moveInput > 0f)
                Player._animator.Play("Sprint_Right");
            else if (Player._moveInput < 0f)
                Player._animator.Play("Sprint_Left");

            // ── Salto desde sprint ───────────────────────────────────────────
            if (Player.jumpPressed && Player._isGrounded)
            {
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.JumpState);
                return;
            }

            // ── Ataque en carrera ────────────────────────────────────────────
            // FIX: usamos attackPressed (nuevo Input System) en lugar de
            //      Input.GetKeyDown(KeyCode.F) (API clásica, inconsistente).
            // FIX: se comprueba _isGrounded para evitar AttackRunState en el aire.
            if (Player.attackPressed && Player._isGrounded)
            {
                fsm.ChangeState(Player.AttackRunState);
                return;
            }

            // ── Dash ─────────────────────────────────────────────────────────
            if (Player.dashPressed && Player._canDashAir)
            {
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.DashState);
                return;
            }

            // ── Caída ────────────────────────────────────────────────────────
            if (!Player._isGrounded && Player._rigidbody2D.linearVelocity.y < 0f)
            {
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.FallingState);
                return;
            }

            // ── Pared ────────────────────────────────────────────────────────
            if (Player._isOnWall)
            {
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.WallSlideState);
                return;
            }
        }

        public override void Exit()
        {
            Player._isSprinting = false;
            Player._audioManager.StopWalk(Player.sfxSource);
            // Restaurar velocidad original al salir del estado
            Player.speed = _originalSpeed;
        }
    }
}