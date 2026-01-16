
using UnityEngine;

namespace _Project.Scripts.Gameplay.PlayerScripts.STATE_MACHINE
{
    public class JumpState : PlayerState
    {
        public JumpState(PlayerStateMachine fsm, Player player) : base(fsm, player) {}
        public Animator Animator { get; private set; }
        
        //Al entrar al estado realizamos el salto.
        public override void Enter()
        {
            Debug.Log("Enter Jump State");
             if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jump);
            Player._rigidbody2D.Jump(Player.jumpForce);
            if (Player._rigidbody2D.linearVelocity.x >= 0)
            {
                Player._animator.Play("Jump_Right");
            }
            else
            {
                Player._animator.Play("Jump_Left");
            }
            
            

        }
        
        public override void LogicUpdate()
        {
            //Si Dasheamos y "_canDashAir" es true, dashea
            if (Player.dashPressed && Player._canDashAir)
            {
                fsm.ChangeState(Player.DashState);
                return;
            }

            //Durante el salto, si soltamos la tecla de salto, este se "cortará" y el salto será más corto o largo dependiendo 
            if (Player.jumpReleased && Player._rigidbody2D.linearVelocity.y > 0)
            {
                Player._jumpCutting = true;
                Player._rigidbody2D.CutJump();
            }

            //Si durante el salto nuestra velocidad pasa a ser 0 o negativa, caemos.
            if (Player._rigidbody2D.linearVelocity.y <= 0)
            {
                fsm.ChangeState(Player.FallingState);
                return;
            }
            
            //Si nos pegamos a una pared pasamos a deslizarnos
            if (Player._isOnWall && Player._rigidbody2D.linearVelocity.y <= 0)
            {
                fsm.ChangeState(Player.WallSlideState);
            }
        }

        //gravedad menor durante el salto para dar un pequeño boost.
        public override void PhysicsUpdate()
        {
            Player.Gravity(3.5f);
        }

        public override void Exit() {}
    }
}
