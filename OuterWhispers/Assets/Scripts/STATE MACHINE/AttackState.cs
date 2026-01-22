using UnityEngine;
using Core.Interfaces;

public class AttackState : PlayerState
{
    private float stateTimer;

    public AttackState(PlayerStateMachine fsm, Player player) : base(fsm, player) { }

    public override void Enter()
    {

        stateTimer = Player.stats.attackCooldown; 
        if (AudioManagerPlayer.Instance != null)
            AudioManagerPlayer.Instance.PlaySFX(AudioManagerPlayer.Instance.punch);
        if (Player._lastInput == -1)
        {
            Player._animator.Play("Attack_Left");
        }
        else
        {
            Player._animator.Play("Attack_Right");
        }
        Player._rigidbody2D.linearVelocity = Vector2.zero;
               
        Vector3 newPosition = new Vector3(Player._lastInput * 0.3f, 0, 0);
        Player.attackPoint.transform.localPosition = newPosition;
        
        Player.attackPoint.gameObject.SetActive(true);
 
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            if (Player._isGrounded)
                fsm.ChangeState(Player.IdleState);
            else
                fsm.ChangeState(Player.FallingState);
        }
    }

    public override void Exit()
    {
        Player.attackPoint.gameObject.SetActive(false);
    }
}