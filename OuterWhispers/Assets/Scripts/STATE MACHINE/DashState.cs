using UnityEngine;


    public class DashState : PlayerState
    {
        private float dashTimer;
        private float dashDuration;


        public DashState(PlayerStateMachine fsm, Player player) : base(fsm, player)
        {
            dashDuration = player._dashDuration;
        }

    public override void Enter()
    {


        if (!Player._isGrounded && !Player._canDashAir) ExitDash();

        Player._audioManager.PlaySFX(Player.dash, Player.sfxSource, 1f);
        if (Player._rigidbody2D.linearVelocity.x >= 0)
        {
            Player._animator.Play("Dash_Right");
        }
        else
        {
            Player._animator.Play("Dash_Left");
        }
        dashTimer = 0f;
        Player._isDashing = true;
        Player._canDashAir = false;

        // Guardamos gravedad
        Player._rigidbody2D.gravityScale = 0f;

        // Dirección del dash
        float direction = Player._moveInput != 0
            ? Mathf.Sign(Player._moveInput)
            : Mathf.Sign(Player.transform.localScale.x);


        Player._rigidbody2D.linearVelocity = new Vector2(
            direction * Player.speed * Player._dashSpeed,
            0f
        );
        
        }

        public override void LogicUpdate()
        {
            dashTimer += Time.deltaTime;

            if (dashTimer >= dashDuration)
            {
                ExitDash();
            }
            
        }

        public override void PhysicsUpdate()
        {
            // Forzar movimiento horizontal puro
            Player._rigidbody2D.linearVelocity = new Vector2(
                Player._rigidbody2D.linearVelocity.x,
                0f
            );
        }


        public override void Exit()
        {
            Player._isDashing = false;
        }

        private void ExitDash()
        {
            // Restaurar gravedad
            Player._rigidbody2D.gravityScale = 3.5f;

            Player._rigidbody2D.linearVelocity = Vector2.zero;

            // Elegir siguiente estado
            if (Player._isGrounded){
                Player._audioManager.PlaySFX(Player.footstep, Player.sfxSource, 1f);
                fsm.ChangeState(Player.IdleState);
            }
            else
                fsm.ChangeState(Player.FallingState);
        }
    }
