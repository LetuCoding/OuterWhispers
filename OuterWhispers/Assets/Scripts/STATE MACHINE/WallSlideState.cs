using UnityEngine;


    public class WallSlideState : PlayerState
    {
        public WallSlideState(PlayerStateMachine fsm, Player player) : base(fsm, player) {}

        public override void Enter()
        {
            //Decimos que estamos Slideando en el Wall
            Player._wallSliding = true;
            
            //Pondríamos la animación de sliding
            //Player._animator.Play("WallSlide");
            
            //Reseteamos el DashAreo
            Player._canDashAir = true;
        }

        public override void LogicUpdate()
        {
            //Si apretamos Dash, dasheamos
            if (Player.dashPressed)
            {
                fsm.ChangeState(Player.DashState);
                return;
            }

            //Si apretamos saltar, hace WallJump y pasamos a JumpState
            if (Player.jumpPressed)
            {
                Player.WallJump();
                fsm.ChangeState(Player.JumpState);
                return;
            }

            //Si nos dejamos caer de la pared, pasamos a FallingState.
            if (!Player._isOnWall)
            {
                fsm.ChangeState(Player.FallingState);
            }
        }

        //Cambiamos la gravedad del jugador para la pared y hacemos que se deslice.
        public override void PhysicsUpdate()
        {
            
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

