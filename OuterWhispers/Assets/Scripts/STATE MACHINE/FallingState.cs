using UnityEngine;


    public class FallingState : PlayerState
    {
        public FallingState(PlayerStateMachine fsm, Player player) : base(fsm, player) {}

        public override void Enter()
        {
            //Aquí pondríamos animación de caída.
            if (Player._rigidbody2D.linearVelocity.x >= 0)
            {
                Player._animator.Play("Fall_Right");
            }
            else
            {
                Player._animator.Play("Fall_Left");
            }
        }

        public override void LogicUpdate()
        {
            
            //Si Dash apretado y Dash en aire es true, cambiamos a DashState
            if (Player.dashPressed && Player._canDashAir)
            {
                fsm.ChangeState(Player.DashState);
                return;
            }

            //Si el jugador está en el suelo cambiamos a Idle
            if (Player._isGrounded)
            {
                fsm.ChangeState(Player.IdleState);
                return;
            }

            //Si el jugador está en la pared cambiamos a estado en pared.
            if (Player._isOnWall)
            {
                fsm.ChangeState(Player.WallSlideState);
            }
        }

        public override void PhysicsUpdate()
        {
            Player.Gravity(4.5f);
        }

        public override void Exit() {}
    }

