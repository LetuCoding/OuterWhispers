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
            ((Keyboard.current.leftShiftKey?.isPressed ?? false) ||
             (Keyboard.current.rightShiftKey?.isPressed ?? false)))
        {
            fsm.ChangeState(Player.SprintState);
            return;
        }

        // Walk
        if (Player._moveInput == 1)
        {
            Player.footsetpPitch = 0.5f;
            Player._animator.Play("Walk_Right");
            Player._audioManager.PlayWalk(Player.footstep, Player.sfxSource, Player.footsetpPitch);
            return;
        }
        else if (Player._moveInput == -1)
        {
            Player.footsetpPitch = 0.5f;
            Player._animator.Play("Walk_Left");
            Player._audioManager.PlayWalk(Player.footstep, Player.sfxSource, Player.footsetpPitch);
            return;
        }

        Player._audioManager.StopWalk(Player.sfxSource);
        Player._animator.Play(Player._lastInput == -1 ? "Idle_Left" : "Idle_Right");

        // Dash / Jump etc... (como lo tengas)
    }

        //Gravedad estandar del personaje
        public override void PhysicsUpdate()
        {
            Player.Gravity(3.5f);
        }



        public override void Exit()
        {
        }
    }
    

