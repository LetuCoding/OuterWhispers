using UnityEngine;

/// <summary>
/// Estado de ataque del jugador. Gestiona un combo de dos golpes:
/// - Primer ataque:  Attack_Right / Attack_Left
/// - Segundo ataque: Attack_Right_Final / Attack_Left_Final
///                   o Attack_Right_Final_2 / Attack_Left_Final_2 (alternado por _comboToggle)
///
/// Si el jugador pulsa atacar de nuevo durante la ventana de combo
/// (<see cref="ComboWindowFraction"/>), encadena el segundo golpe al terminar el primero.
/// </summary>
public class AttackState : PlayerState
{
    // =========================================================================
    // ESTADO INTERNO
    // =========================================================================

    private float _stateTimer;
    private bool  _comboQueued;  // El jugador pulsó atacar durante el primer golpe
    private bool  _isSecondHit;  // Estamos ejecutando el segundo golpe del combo
    private bool  _comboToggle;  // Alterna entre _Final y _Final_2 en cada combo

    /// <summary>
    /// Fracción del cooldown durante la cual se acepta la entrada de combo.
    /// 0.6 = se puede encadenar durante el primer 60 % de la animación.
    /// </summary>
    private const float ComboWindowFraction = 0.6f;

    public AttackState(PlayerStateMachine fsm, Player player) : base(fsm, player) { }

    // =========================================================================
    // FSM
    // =========================================================================

    public override void Enter()
    {
        Debug.Log("Entering Attack State");

        _stateTimer  = Player.stats.attackCooldown;
        _comboQueued = false;
        _isSecondHit = false;

        Player._audioManager.PlaySFX(Player.punch, Player.sfxSource, 1f);

        PlayFirstHitAnim();
        SetupHitbox();
    }

    public override void LogicUpdate()
    {
        _stateTimer -= Time.deltaTime;

        // Ventana de combo: acepta input solo durante el primer golpe
        if (!_isSecondHit && !_comboQueued)
        {
            float comboWindowEnd = Player.stats.attackCooldown * (1f - ComboWindowFraction);
            if (_stateTimer > comboWindowEnd && Player.attackPressed)
                _comboQueued = true;
        }

        if (_stateTimer <= 0f)
        {
            if (_isSecondHit)
            {
                // Segundo golpe completado → salir del estado
                fsm.ChangeState(Player._isGrounded ? Player.IdleState : Player.FallingState);
            }
            else if (_comboQueued)
            {
                // Encadenar segundo golpe
                ExecuteSecondHit();
            }
            else
            {
                // Sin combo → salir
                fsm.ChangeState(Player._isGrounded ? Player.IdleState : Player.FallingState);
            }
        }
    }

    /// <summary>
    /// Frena al jugador cada frame mientras ataca.
    /// Sin esto, FixedUpdate de Player sigue aplicando _moveInput
    /// y el personaje se desliza durante el golpe.
    /// </summary>
    public override void PhysicsUpdate()
    {
        Player._rigidbody2D.linearVelocity = new Vector2(0f, Player._rigidbody2D.linearVelocity.y);
    }

    public override void Exit()
    {
        Player.attackPoint.gameObject.SetActive(false);
    }

    // =========================================================================
    // PRIVADO
    // =========================================================================

    private void PlayFirstHitAnim()
    {
        Player._animator.Play(Player._lastInput < 0 ? "Attack_Left" : "Attack_Right");
    }

    private void ExecuteSecondHit()
    {
        _isSecondHit = true;
        _stateTimer  = Player.stats.attackCooldown;
        _comboToggle = !_comboToggle;

        Player._audioManager.PlaySFX(Player.punch, Player.sfxSource, 1f);

        // Alterna entre Attack_Right_Final y Attack_Right_Final_2 en cada combo
        string suffix =  "Final_2";
        string dir    = Player._lastInput < 0 ? "Left" : "Right";
        Player._animator.Play($"Attack_{dir}_{suffix}");

        // Desactivar y reactivar el hitbox para registrar el segundo golpe
        Player.attackPoint.gameObject.SetActive(false);
        SetupHitbox();
    }

    private void SetupHitbox()
    {
        Player.attackPoint.localPosition = new Vector3(Player._lastInput * 0.3f, 0f, 0f);
        Player.attackPoint.localScale    = new Vector3(
            Player.stats.attackRange.x,
            Player.stats.attackRange.y,
            0f
        );
        Player.attackPoint.gameObject.SetActive(true);
    }
}