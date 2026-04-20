using UnityEngine;

/// <summary>
/// Estado de dash del jugador. Desactiva la gravedad, aplica velocidad horizontal
/// pura durante <see cref="Player._dashDuration"/> segundos y restaura la gravedad al salir.
/// </summary>
public class DashState : PlayerState
{
    private float _dashTimer;

    // FIX: la duración se lee de Player en Enter() en lugar de capturarla en el
    //      constructor, de modo que si _dashDuration cambia en runtime se respeta.

    public DashState(PlayerStateMachine fsm, Player player) : base(fsm, player) { }

    public override void Enter()
    {
        Debug.Log("Entering Dash State");

        // Guard: no dashear en el aire si ya se usó el dash aéreo
        if (!Player._isGrounded && !Player._canDashAir)
        {
            ExitDash();
            return;
        }

        Player._audioManager.PlaySFX(Player.dash, Player.sfxSource, 1f);

        // Animación según dirección actual de la velocidad
        Player._animator.Play(Player._rigidbody2D.linearVelocity.x >= 0 ? "Dash_Right" : "Dash_Left");

        _dashTimer         = 0f;
        Player._isDashing  = true;
        Player._canDashAir = false;

        // Sin gravedad durante el dash para trayectoria 100 % horizontal
        Player._rigidbody2D.gravityScale = 0f;

        // Dirección: input actual > fallback a escala del transform
        float direction = Player._moveInput != 0f
            ? Mathf.Sign(Player._moveInput)
            : Mathf.Sign(Player.transform.localScale.x);

        Player._rigidbody2D.linearVelocity = new Vector2(
            direction * Player.speed * Player._dashSpeed,
            0f
        );
    }

    public override void LogicUpdate()
    {
        _dashTimer += Time.deltaTime;

        // FIX: leemos _dashDuration en cada frame por si cambia en runtime
        if (_dashTimer >= Player._dashDuration)
            ExitDash();
    }

    public override void PhysicsUpdate()
    {
        // Mantener velocidad Y en 0 por si la física intenta introducir caída
        Player._rigidbody2D.linearVelocity = new Vector2(
            Player._rigidbody2D.linearVelocity.x,
            0f
        );
    }

    public override void Exit()
    {
        Player._isDashing = false;
    }

    // -------------------------------------------------------------------------

    /// <summary>
    /// Restaura la gravedad, para el Rigidbody y cambia al estado correcto.
    /// </summary>
    private void ExitDash()
    {
        // FIX: gravedad restaurada a la misma constante que usa IdleState/JumpState
        //      (3.5f). Podría parametrizarse en PlayerStats si se necesita flexibilidad.
        Player._rigidbody2D.gravityScale   = 3.5f;
        Player._rigidbody2D.linearVelocity = Vector2.zero;

        if (Player._isGrounded)
        {
            Player._audioManager.PlaySFX(Player.footstep, Player.sfxSource, 1f);
            fsm.ChangeState(Player.IdleState);
        }
        else
        {
            fsm.ChangeState(Player.FallingState);
        }
    }
}