using System.Collections;
using _Project.Scripts.Gameplay.PlayerScripts.STATE_MACHINE;
using Interfaces;
using InventoryScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Zenject;

/// <summary>
/// Componente principal del jugador. Gestiona input, físicas, checks de colisión,
/// audio y delega la lógica de comportamiento a la máquina de estados.
/// </summary>
public class Player : MonoBehaviour, IEffectTarget, IPlayer
{
    // =========================================================================
    // COMPONENTS & INPUT
    // =========================================================================

    /// <summary>Mapa de acciones del nuevo Input System.</summary>
    public PlayerInputActions _playerInputActions;

    public Rigidbody2D     _rigidbody2D;
    public Animator        _animator;
    public IAudioManager   _audioManager;
    public HealthComponent _healthComponent;
    public DeathScreen     _deathScreen;

    private Inventory _inventory;

    /// <summary>Inventario del jugador (acceso de sólo lectura externo).</summary>
    public Inventory Inventory => _inventory;

    // --- Estado de movimiento ---
    public float _moveInput;        // Eje horizontal del frame actual (-1, 0, 1)
    public float _lastInput;        // Última dirección registrada (no se pone a 0)
    public bool  _isGrounded;
    public bool  _jumpCutting;
    public bool  _canDashAir;
    public bool  _isDashing;
    public bool  _isSprinting;
    public bool  _isAttackSliding;

    // --- Flags de input (seteados en Update, consumidos por los estados) ---
    public bool jumpPressed;
    public bool jumpReleased;
    public bool dashPressed;
    public bool attackPressed;          // FIX: antes nunca se asignaba
    public bool shiftPressedThisFrame;
    public bool shiftHeld;

    // --- Estado en pared ---
    public bool _isOnWall;
    public bool _isOnLeftWall;
    public bool _isOnRightWall;
    public bool _wallJumping;
    public bool _wallSliding;

    // =========================================================================
    // ATTACK
    // =========================================================================

    [Header("Attack Settings")]
    public PlayerStats stats;
    public Transform   attackPoint;
    public LayerMask   enemyLayer;

    /// <summary>Estado de ataque, accedido por otros estados para combos.</summary>
    public AttackState AttackState { get; private set; }

    // =========================================================================
    // GROUND CHECK
    // =========================================================================

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float     groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    // =========================================================================
    // AUDIO
    // =========================================================================

    [Header("Audio Sources")]
    [SerializeField] public AudioSource sfxSource;
    [SerializeField] public AudioSource itemSource;
    [SerializeField] public AudioSource ambienceSource;
    [SerializeField] public AudioSource musicSource;

    [Header("SFX Clips")]
    public AudioClip footstep;
    public AudioClip dash;
    public AudioClip jump;
    public AudioClip slide;
    public AudioClip punch;
    public AudioClip die;
    public AudioClip damage;
    public AudioClip heal;
    public AudioClip sadMusic;

    [Header("Audio Pitch")]
    public float footsetpPitch = 0.5f;

    // =========================================================================
    // WALL CHECK
    // =========================================================================

    [SerializeField] private Transform _wallCheckLeft;
    [SerializeField] private Transform _wallCheckRight;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float     wallCheckRadius = 0.1f;

    /// <summary>Escala de gravedad durante el deslizamiento en pared.</summary>
    // FIX: [SerializeField] no funciona en propiedades auto-implementadas;
    //      se convierte en campo serializable con getter explícito.
    [SerializeField] private float _wallSlideGravity = 1f;
    public float wallSlideGravity => _wallSlideGravity;

    [SerializeField] private float wallJumpForceX;
    [SerializeField] private float wallJumpForceY;

    private readonly float _wallJumpLockTime = 0.125f;

    // =========================================================================
    // STATE MACHINE
    // =========================================================================

    #region StateMachine

    public PlayerStateMachine StateMachine { get; private set; }

    public IdleState      IdleState      { get; private set; }
    public JumpState      JumpState      { get; private set; }
    public SprintState    SprintState    { get; private set; }
    public FallingState   FallingState   { get; private set; }
    public AttackRunState AttackRunState { get; private set; }
    public WallSlideState WallSlideState { get; private set; }
    public DashState      DashState      { get; private set; }

    private bool _isDead;

    #endregion

    // =========================================================================
    // CONFIGURATION
    // =========================================================================

    [Header("Movement Settings")]
    public float speed;
    public float jumpForce;

    [Header("Dash Settings")]
    [SerializeField] public float _dashDuration;
    [SerializeField] public float _dashSpeed;

    [Header("UI")]
    [SerializeField] private GameObject uiOptions;
    private bool _isPaused;

    [Header("Save System")]
    public UnityEvent OnLoadGame;

    // =========================================================================
    // UNITY LIFECYCLE
    // =========================================================================

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        _rigidbody2D      = GetComponent<Rigidbody2D>();
        _animator         = GetComponent<Animator>();
        _healthComponent  = GetComponent<HealthComponent>();
        _inventory        = GetComponent<Inventory>();

        // Inicialización de la máquina de estados y sus estados
        StateMachine  = new PlayerStateMachine();
        IdleState     = new IdleState(StateMachine, this);
        JumpState     = new JumpState(StateMachine, this);
        WallSlideState= new WallSlideState(StateMachine, this);
        FallingState  = new FallingState(StateMachine, this);
        DashState     = new DashState(StateMachine, this);
        AttackState   = new AttackState(StateMachine, this);
        SprintState   = new SprintState(StateMachine, this);
        AttackRunState= new AttackRunState(StateMachine, this);
    }

    /// <summary>Inyección de dependencia de Zenject para el AudioManager.</summary>
    [Inject]
    public void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    private void Start()
    {
        // Estado inicial de la FSM
        StateMachine.Initialize(IdleState);
        _deathScreen.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isDead) return;

        // Leer todos los inputs en un único lugar; los estados los consumen.
        jumpPressed   = _playerInputActions.Player.Jump.WasPressedThisFrame();
        jumpReleased  = _playerInputActions.Player.Jump.WasReleasedThisFrame();
        dashPressed   = _playerInputActions.Player.Dash.WasPressedThisFrame();
        attackPressed = _playerInputActions.Player.Attack.WasPressedThisFrame(); // FIX: asignación que faltaba

        _moveInput = _playerInputActions.Player.Move.ReadValue<Vector2>().x;

        shiftPressedThisFrame =
            (Keyboard.current.leftShiftKey?.wasPressedThisFrame  ?? false) ||
            (Keyboard.current.rightShiftKey?.wasPressedThisFrame ?? false);

        shiftHeld =
            (Keyboard.current.leftShiftKey?.isPressed  ?? false) ||
            (Keyboard.current.rightShiftKey?.isPressed ?? false);

        if (_moveInput != 0)
            _lastInput = _moveInput;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePauseMenu();
        if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
            TogglePauseMenu();

        GroundCheck();
        WallCheck();
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        if (_isDead) return;

        // FIX: bloque duplicado eliminado. Una sola comprobación para ambos flags.
        if (_isDashing || _isAttackSliding)
        {
            StateMachine.CurrentState.PhysicsUpdate();
            return;
        }

        if (_moveInput != 0)
        {
            _rigidbody2D.linearVelocity = new Vector2(_moveInput * speed, _rigidbody2D.linearVelocity.y);
        }
        else if (!_isGrounded)
        {
            // En el aire sin input: no cancelar inercia horizontal (permite coyote-time y wall-jumps suaves)
        }
        else
        {
            // En el suelo sin input: frenar en seco
            _rigidbody2D.linearVelocity = new Vector2(0f, _rigidbody2D.linearVelocity.y);
        }

        StateMachine.CurrentState.PhysicsUpdate();
    }

    // =========================================================================
    // CHECKS DE COLISIÓN
    // =========================================================================

    /// <summary>Detecta si el jugador está tocando el suelo y resetea flags relacionados.</summary>
    private void GroundCheck()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, groundCheckRadius, groundLayer);

        if (_isGrounded)
        {
            _jumpCutting = false;
            _canDashAir  = true;
        }
    }

    /// <summary>Detecta si el jugador está en contacto con una pared (izquierda o derecha).</summary>
    private void WallCheck()
    {
        _isOnLeftWall  = Physics2D.OverlapCircle(_wallCheckLeft.position,  wallCheckRadius, wallLayer);
        _isOnRightWall = Physics2D.OverlapCircle(_wallCheckRight.position, wallCheckRadius, wallLayer);
        _isOnWall      = (_isOnLeftWall || _isOnRightWall) && !_isGrounded;
    }

    // =========================================================================
    // MOVIMIENTO
    // =========================================================================

    /// <summary>Aplica una escala de gravedad personalizada al Rigidbody2D.</summary>
    /// <param name="gravity">Valor de gravityScale deseado.</param>
    public void Gravity(float gravity)
    {
        _rigidbody2D.gravityScale = gravity;
    }

    /// <summary>
    /// Ejecuta un wall jump: calcula la dirección según la pared tocada
    /// y aplica impulso; bloquea el input brevemente para evitar reentrar en la pared.
    /// </summary>
    public void WallJump()
    {
        _wallJumping = true;
        _wallSliding = false;
        _isOnWall    = false;

        // El jugador salta hacia el lado contrario de la pared que está tocando
        float jumpDirection = _isOnLeftWall ? 1f : -1f;

        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.linearVelocity = new Vector2(jumpDirection * wallJumpForceX, wallJumpForceY);

        StartCoroutine(WallJumpLock());
    }

    /// <summary>
    /// Bloquea el input horizontal durante <see cref="_wallJumpLockTime"/> segundos
    /// para evitar que el jugador vuelva inmediatamente a la pared tras saltar.
    /// </summary>
    private IEnumerator WallJumpLock()
    {
        _wallJumping = true;
        _rigidbody2D.gravityScale = 7.5f;

        float elapsed = 0f;
        while (elapsed < _wallJumpLockTime)
        {
            _moveInput = 0f;
            elapsed   += Time.deltaTime;
            yield return null;
        }

        _wallJumping = false;
    }

    // =========================================================================
    // PAUSA
    // =========================================================================

    private void TogglePauseMenu()
    {
        _isPaused = !_isPaused;
        _playerInputActions.Player.Disable();
        if (uiOptions != null)
        {
            uiOptions.SetActive(_isPaused);
            if (_isPaused)
            {
                OptionsMenuManager options = uiOptions.GetComponent<OptionsMenuManager>();
                if (options != null)
                {
                    options.SetInitialFocus();
                }
            }
        }
    }

    public void UnPauseMenu()
    {
        if (_playerInputActions != null)
        {
            _playerInputActions.Player.Enable();
        }
    }

    // =========================================================================
    // FREEZE / UNFREEZE
    // =========================================================================

    /// <summary>
    /// Congela al jugador: desactiva el input y convierte el Rigidbody en estático.
    /// Se usa al morir o en cinemáticas.
    /// </summary>
    public void FreezePlayer()
    {
        _playerInputActions?.Disable();

        if (_rigidbody2D != null)
        {
            _rigidbody2D.linearVelocity  = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
            _rigidbody2D.bodyType        = RigidbodyType2D.Static;
        }
    }

    /// <summary>
    /// Descongela al jugador: reactiva el input y restaura el Rigidbody dinámico.
    /// </summary>
    public void UnfreezePlayer()
    {
        _playerInputActions?.Enable();

        if (_rigidbody2D != null)
        {
            _rigidbody2D.bodyType        = RigidbodyType2D.Dynamic;
            _rigidbody2D.linearVelocity  = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
        }
    }

    // =========================================================================
    // DAÑO, MUERTE Y CURACIÓN
    // =========================================================================

    /// <summary>Reproduce el efecto visual y de sonido al recibir daño.</summary>
    public void TakeDamage()
    {
        StartCoroutine(HitEffect());
    }

    /// <summary>Parpadeo rojo de 0.25 s al ser golpeado.</summary>
    private IEnumerator HitEffect()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        _audioManager.PlaySFX(damage, sfxSource, 1f);

        if (sr != null) sr.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        if (sr != null) sr.color = Color.white;
    }

    /// <summary>
    /// Ejecuta la secuencia de muerte: animación, freeze, audio y pantalla de muerte.
    /// Protegido contra doble llamada con <c>_isDead</c>.
    /// </summary>
    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        _animator.Play(_lastInput >= 0 ? "Die_Right" : "Die_Left");

        FreezePlayer();
        _audioManager.PlaySFX(die, sfxSource, 1f);
        _audioManager.StopAmbience(ambienceSource);
        _audioManager.PlayMusic(sadMusic, musicSource);

        _deathScreen.gameObject.SetActive(true);
        _deathScreen.FadeToBlackAndShowMessage();
    }

    /// <summary>Cura al jugador la cantidad indicada y reproduce el sonido de curación.</summary>
    /// <param name="amount">Cantidad de vida a restaurar.</param>
    public void Heal(float amount)
    {
        _audioManager.PlaySFX(heal, itemSource, 1f);
        Debug.Log($"Healing {amount}");
    }

    // =========================================================================
    // SAVE SYSTEM
    // =========================================================================

    /// <summary>Lanza el evento <see cref="OnLoadGame"/> para notificar al sistema de guardado.</summary>
    public void LoadSavedGame()
    {
        OnLoadGame?.Invoke();
    }

    // =========================================================================
    // GIZMOS (sólo editor)
    // =========================================================================

    private void OnDrawGizmosSelected()
    {
        if (_groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundCheck.position,    groundCheckRadius);
        Gizmos.DrawWireSphere(_wallCheckLeft.position,  wallCheckRadius);
        Gizmos.DrawWireSphere(_wallCheckRight.position, wallCheckRadius);
        Gizmos.DrawWireCube(attackPoint.position,       stats.attackRange);
    }
}