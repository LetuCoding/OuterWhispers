using UnityEngine;

namespace _Project.Scripts.Gameplay.PlayerScripts.STATE_MACHINE
{
    /// <summary>
    /// Estado de salto del jugador. Aplica el impulso en Enter() y gestiona
    /// el jump-cut (soltar espacio = salto más corto) y la transición a caída.
    /// </summary>
    public class JumpState : PlayerState
    {
        public JumpState(PlayerStateMachine fsm, Player player) : base(fsm, player) { }

        public override void Enter()
        {
            Debug.Log("Enter Jump State");

            Player._audioManager.PlaySFX(Player.jump, Player.sfxSource, 1f);

            // Extensión que limpia la Y antes de aplicar jumpForce para consistencia
            Player._rigidbody2D.Jump(Player.jumpForce);

            Player._animator.Play(Player._lastInput >= 0 ? "Jump_Right" : "Jump_Left");
        }

        public override void LogicUpdate()
        {
            // Dash aéreo durante el salto
            if (Player.dashPressed && Player._canDashAir)
            {
                fsm.ChangeState(Player.DashState);
                return;
            }

            // Jump-cut: soltar el botón de salto recorta la altura
            if (Player.jumpReleased && Player._rigidbody2D.linearVelocity.y > 0f)
            {
                Player._jumpCutting = true;
                Player._rigidbody2D.CutJump();
            }

            // Velocidad Y negativa o nula → empezar a caer
            if (Player._rigidbody2D.linearVelocity.y <= 0f)
            {
                fsm.ChangeState(Player.FallingState);
                return;
            }

            // Contacto con pared mientras sube (raro pero posible)
            if (Player._isOnWall && Player._rigidbody2D.linearVelocity.y <= 0f)
            {
                fsm.ChangeState(Player.WallSlideState);
            }
        }

        /// <summary>Gravedad moderada durante el ascenso para un arco de salto satisfactorio.</summary>
        public override void PhysicsUpdate()
        {
            Player.Gravity(3.5f);
        }

        public override void Exit() { }
    }
}