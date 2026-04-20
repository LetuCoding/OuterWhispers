using UnityEngine;

namespace _Project.Scripts.Gameplay.PlayerScripts.STATE_MACHINE
{
    /// <summary>
    /// Estado de ataque en carrera. Consta de dos fases:
    /// <list type="bullet">
    ///   <item><term>Slide</term><description>El jugador se desliza hacia delante a alta velocidad.</description></item>
    ///   <item><term>Strike</term><description>Se detiene y ejecuta el golpe final antes de volver a Idle/Falling.</description></item>
    /// </list>
    /// </summary>
    public class AttackRunState : PlayerState
    {
        private enum Phase { Slide, Strike }

        private Phase _phase;
        private float _timer;
        private int   _dir; // -1 = izquierda, +1 = derecha

        // FIX: constantes convertidas en campos [SerializeField] para poder
        //      ajustarse desde el Inspector sin recompilar.
        [SerializeField] private float _slideSpeed = 12f;
        [SerializeField] private float _slideTime  = 0.5f;
        [SerializeField] private float _strikeTime = 0.5f;

        public AttackRunState(PlayerStateMachine fsm, Player player) : base(fsm, player) { }

        public override void Enter()
        {
            Debug.Log("Entering AttackRun State");

            _dir = Player._lastInput < 0 ? -1 : 1;

            Player._isAttackSliding = true;

            Player._audioManager.PlaySFX(Player.punch, Player.sfxSource, 1f);

            Player._animator.Play(_dir < 0 ? "Sprint_Attack_Left" : "Sprint_Attack_Right");

            // Posicionar hitbox
            Player.attackPoint.localPosition = new Vector3(_dir * 0.3f, 0f, 0f);
            Player.attackPoint.localScale    = new Vector3(
                Player.stats.attackRange.x,
                Player.stats.attackRange.y,
                0f
            );
            Player.attackPoint.gameObject.SetActive(true);

            _phase = Phase.Slide;
            _timer = _slideTime;

            Player._rigidbody2D.linearVelocity = new Vector2(
                _dir * _slideSpeed,
                Player._rigidbody2D.linearVelocity.y
            );
        }

        public override void LogicUpdate()
        {
            _timer -= Time.deltaTime;

            switch (_phase)
            {
                case Phase.Slide:
                    if (_timer <= 0f)
                    {
                        _phase = Phase.Strike;
                        _timer = _strikeTime;

                        // Parar en seco para el golpe final
                        Player._rigidbody2D.linearVelocity = new Vector2(
                            0f,
                            Player._rigidbody2D.linearVelocity.y
                        );

                        Player._animator.Play(
                            _dir < 0 ? "Sprint_Attack_Left_Recover" : "Sprint_Attack_Right_Recover"
                        );
                    }
                    // FIX: si el jugador cae de una plataforma durante el slide, salir correctamente
                    else if (!Player._isGrounded && Player._rigidbody2D.linearVelocity.y < 0f)
                    {
                        fsm.ChangeState(Player.FallingState);
                    }
                    break;

                case Phase.Strike:
                    if (_timer <= 0f)
                        fsm.ChangeState(Player._isGrounded ? Player.IdleState : Player.FallingState);
                    break;
            }
        }

        public override void PhysicsUpdate()
        {
            // Mantener velocidad de slide constante durante la fase deslizante
            if (_phase == Phase.Slide)
            {
                Player._rigidbody2D.linearVelocity = new Vector2(
                    _dir * _slideSpeed,
                    Player._rigidbody2D.linearVelocity.y
                );
            }
        }

        public override void Exit()
        {
            Player._isAttackSliding = false;
            Player.attackPoint.gameObject.SetActive(false);

            // Evitar velocidad residual al salir
            Player._rigidbody2D.linearVelocity = new Vector2(0f, Player._rigidbody2D.linearVelocity.y);
        }
    }
}