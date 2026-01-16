using UnityEngine;


    public class IdleState : PlayerState
    {
    public IdleState(PlayerStateMachine fsm, Player player) : base(fsm, player) {}
    public override void Enter()
    {
        Debug.Log("Entering Idle State");
        if (Player._lastInput == -1)
        {
            Player._animator.Play("Idle_Left");
        }
        else
        {
            Player._animator.Play("Idle_Right");
        }
        Player._jumpCutting = false;
    }

    public override void LogicUpdate()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayWalk();
        if (Player._moveInput == 1)
        {
            Player._animator.Play("Walk_Right");

        }
        else if (Player._moveInput == -1)
        {
            Player._animator.Play("Walk_Left");


        }


        

        else if (Player._moveInput == 0)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopWalk();
            if (Player._lastInput == -1)
            {
                Player._animator.Play("Idle_Left");
            }
            else
            {
                Player._animator.Play("Idle_Right");
            }

        }
            
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
    

