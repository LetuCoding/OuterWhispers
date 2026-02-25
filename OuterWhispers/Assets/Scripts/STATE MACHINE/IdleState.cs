using UnityEngine;
using UnityEngine.InputSystem;
 
 
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
        if (Player._moveInput != 0 &&
            (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed))
        {
            Player.footsetpPitch = 0.7f;
            Player._audioManager.PlayWalk(Player.footstep,Player.sfxSource,Player.footsetpPitch);
            fsm.ChangeState(Player.SprintState);
            return;
        }
        else if (Player._moveInput == 1)
        {
            Player.footsetpPitch = 0.5f;
            Player._animator.Play("Walk_Right");
            Player._audioManager.PlayWalk(Player.footstep,Player.sfxSource,Player.footsetpPitch);
        }
        else if (Player._moveInput == -1)
        {
            Player.footsetpPitch = 0.5f;
            Player._animator.Play("Walk_Left");
            Player._audioManager.PlayWalk(Player.footstep,Player.sfxSource,Player.footsetpPitch);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            fsm.ChangeState(Player.AttackState);
        }
 
 
        else if (Player._moveInput == 0)
        {
            Player._audioManager.StopWalk(Player.sfxSource);
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
            Player._audioManager.StopWalk(Player.sfxSource);
            fsm.ChangeState(Player.DashState);
            return;
        }
 
 
        //Si estamos en el suelo y apretamos Jump, Saltaremos
        if (Player.jumpPressed && Player._isGrounded)
        {
            Player._audioManager.StopWalk(Player.sfxSource);
            fsm.ChangeState(Player.JumpState);
            return;
        }
        
        //Si estamos en el suelo y sprintando haremos un salto más largo
        if (Player.jumpPressed && Player._isSprinting)
        {
            Player._audioManager.StopWalk(Player.sfxSource);
            fsm.ChangeState(Player.JumpState);
            return;
        }
        
        
 
        //Si el jugador no está en el suelo y su velocidad es inferior a 0, pasamos a caer.
        if (!Player._isGrounded && Player._rigidbody2D.linearVelocity.y < 0f)
        {
            fsm.ChangeState(Player.FallingState);
        }
        if (Player._moveInput != 0 &&
            (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed))
        {
            fsm.ChangeState(Player.SprintState);
            return;
        }
 
    }
 
    //Gravedad estandar del personaje
    public override void PhysicsUpdate()
    {
        Player.Gravity(3.5f);
    }
 
 
 
    public override void Exit()
    {
        Player._audioManager.StopWalk(Player.sfxSource);
    }
}