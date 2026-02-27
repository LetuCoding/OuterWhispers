using UnityEngine;

namespace _Project.Scripts.Gameplay.PlayerScripts.STATE_MACHINE
{
    public class AttackRunState : PlayerState
    {
        private enum Phase { Slide, Strike }

        private Phase _phase;
        private float _timer;

        // ===== TUNING =====
        private const float SlideSpeed = 12f;      // velocidad del deslizamiento
        private const float SlideTime = 0.5f;    // tiempo deslizando (ajusta para “metros”)
        private const float StrikeTime = 0.5f;   // tiempo parado ejecutando golpe

        private int _dir; // -1 izquierda, +1 derecha

        public AttackRunState(PlayerStateMachine fsm, Player player) : base(fsm, player) { }

        public override void Enter()
        {
            Debug.Log("Entering AttackRun State");

            // Dirección por último input
            _dir = (Player._lastInput < 0) ? -1 : 1;

            // Bloquea movimiento normal
            Player._isAttackSliding = true;

            // SFX
            Player._audioManager.PlaySFX(Player.punch, Player.sfxSource, 1f);

            // Anim (puedes usar una anim distinta si tienes)
            if (_dir < 0) Player._animator.Play("Sprint_Attack_Left");
            else         Player._animator.Play("Sprint_Attack_Right");

            // Hitbox / attackPoint (igual que tu AttackState)
            Vector3 newPosition = new Vector3(_dir * 0.3f, 0, 0);
            Player.attackPoint.transform.localPosition = newPosition;
            Player.attackPoint.localScale = new Vector3(Player.stats.attackRange.x, Player.stats.attackRange.y, 0);
            Player.attackPoint.gameObject.SetActive(true);

            // Arranca en fase slide
            _phase = Phase.Slide;
            _timer = SlideTime;

            // Aplicamos velocidad inicial de slide
            Player._rigidbody2D.linearVelocity = new Vector2(_dir * SlideSpeed, Player._rigidbody2D.linearVelocity.y);
        }

        public override void LogicUpdate()
        {
            _timer -= Time.deltaTime;

            if (_phase == Phase.Slide)
            {
                // Si termina el slide, pasamos a fase golpe/parada
                if (_timer <= 0f)
                {
                    _phase = Phase.Strike;
                    _timer = StrikeTime;

                    // Parar en seco para “golpe en carrera”
                    Player._rigidbody2D.linearVelocity = new Vector2(0f, Player._rigidbody2D.linearVelocity.y);

                    // Si quieres una anim específica del golpe final:
                    if (_dir < 0) Player._animator.Play("Sprint_Attack_Left_Recover");
                    else         Player._animator.Play("Sprint_Attack_Right_Recover");
                }
            }
            else // Phase.Strike
            {
                if (_timer <= 0f)
                {
                    // Fin del estado
                    if (Player._isGrounded) fsm.ChangeState(Player.IdleState);
                    else fsm.ChangeState(Player.FallingState);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            // Durante slide, mantenemos la velocidad constante (sin que la física la degrade)
            if (_phase == Phase.Slide)
            {
                Player._rigidbody2D.linearVelocity = new Vector2(_dir * SlideSpeed, Player._rigidbody2D.linearVelocity.y);
            }

            // Ajusta gravedad si quieres que el slide sea más “pegado al suelo”
            // Player.Gravity(3.5f);
        }

        public override void Exit()
        {
            Player._isAttackSliding = false;
            Player.attackPoint.gameObject.SetActive(false);

            // Por seguridad, no dejar velocidad residual
            Player._rigidbody2D.linearVelocity = new Vector2(0f, Player._rigidbody2D.linearVelocity.y);
        }
    }
}