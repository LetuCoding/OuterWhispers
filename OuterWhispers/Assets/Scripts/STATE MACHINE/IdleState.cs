using UnityEngine;


    public class IdleState : PlayerState
    {
        public IdleState(PlayerStateMachine fsm, Player player) : base(fsm, player) {}

        //Al entrar reseteamos el JumpCutting
        public override void Enter()
        {
            Debug.Log("Entering Idle State");
            Player._animator.Play("Idle");
            Player._jumpCutting = false;
        }

        public override void LogicUpdate()
        {
            
            //Apretamos Dash y lo hace
            if (Player.dashPressed)
            {
                fsm.ChangeState(Player.DashState);
                return;
            }
            
            
            //Si estamos en el suelo y apretamos Dash, Dasheamos
            if (Player.jumpPressed && Player._isGrounded)
            {
                fsm.ChangeState(Player.JumpState);
                return;
            }

            //Si el jugador no está en el suelo y su velocidad es inferior a 0, pasamos a caer.
            if (!Player._isGrounded && Player._rigidbody2D.linearVelocity.y < 0f)
            {
                fsm.ChangeState(Player.FallingState);
            }
        }

        //Gravedad estandar del personaje
        public override void PhysicsUpdate()
        {
            Player.Gravity(3.5f);
        }

        public override void Exit() {}
    }

