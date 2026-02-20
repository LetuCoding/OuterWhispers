using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Gameplay.PlayerScripts.STATE_MACHINE
{
    public class SprintState : PlayerState
    {
        private float _originalSpeed;
        private const float SprintMultiplier = 2f;

        public SprintState(PlayerStateMachine fsm, Player player) : base(fsm, player) { }

        public override void Enter()
        {
            Debug.Log("Entering Sprint State");
            _originalSpeed = Player.speed;
            Player.speed = _originalSpeed * SprintMultiplier;
            Player.footsetpPitch = 1f;
            Player._audioManager.PlayWalk(Player.footstep, Player.sfxSource, Player.footsetpPitch);
        }

        public override void LogicUpdate()
        {
            // SHIFT mantenido (izq o dcha)
            bool shiftHeld =
                (Keyboard.current.leftShiftKey != null && Keyboard.current.leftShiftKey.isPressed) ||
                (Keyboard.current.rightShiftKey != null && Keyboard.current.rightShiftKey.isPressed);

            // Si suelta SHIFT -> salir del sprint
            if (!shiftHeld)
            {
                Player.footsetpPitch = 0.5f;
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.IdleState);
                return;
            }

            // Si no se mueve, no tiene sentido seguir sprintando (opcional pero recomendado)
            if (Mathf.Approximately(Player._moveInput, 0f))
            {
                Player.footsetpPitch = 0.5f;
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.IdleState);
                return;
            }
            if (Player._moveInput == 1)
            {
                Player._animator.Play("Sprint_Right");
            }
            else if (Player._moveInput == -1)
            {
                Player._animator.Play("Sprint_Left");
            }
            // Si saltas desde suelo
            if (Player.jumpPressed && Player._isGrounded)
            {
                Player.footsetpPitch = 0.5f;
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.JumpState);
                return;
            }

            // Si dasheas (tu Player ya gestiona _isDashing para físicas)
            if (Player.dashPressed && Player._canDashAir)
            {
                Player.footsetpPitch = 0.5f;
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.DashState);
                return;
            }

            // Si te vas al aire cayendo
            if (!Player._isGrounded && Player._rigidbody2D.linearVelocity.y < 0f)
            {
                Player.footsetpPitch = 0.5f;
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.FallingState);
                return;
            }

            // Si entras a pared (si tu lógica lo requiere)
            if (Player._isOnWall)
            {
                Player.footsetpPitch = 0.5f;
                Player._audioManager.StopWalk(Player.sfxSource);
                fsm.ChangeState(Player.WallSlideState);
                return;
            }
        }

        public override void Exit()
        {
            // Restauramos la speed original al salir del sprint
            Player.speed = _originalSpeed;
        }
    }
}
