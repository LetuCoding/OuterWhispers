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
    public float _moveInput;
    public float _lastInput;
    public bool  _isGrounded;
    public bool  _jumpCutting;
    public bool  _canDashAir;
    public bool  _isDashing;
    public bool  _isSprinting;
    public bool  _isAttackSliding;

    // --- Flags de input ---
    public bool jumpPressed;
    public bool jumpReleased;
    public bool dashPressed;
    public bool attackPressed;
    public bool shiftPressedThisFrame;
    public bool shiftHeld;
    public bool inventoryPressed;

    // --- Estado en pared ---
    public bool _isOnWall;
    public bool _isOnLeftWall;
    public bool _isOnRightWall;
    public bool _wallJumping;
    public bool _wallSliding;
    public bool canMove = true;

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
    private bool _isInventoryOpen;

    [Header("Save System")]
    public UnityEvent OnLoadGame;

    // =========================================================================
    // UNITY LIFECYCLE
    // =========================================================================

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();

        _rigidbody2D     = GetComponent<Rigidbody2D>();
        _animator        = GetComponent<Animator>();
        _healthComponent = GetComponent<HealthComponent>();
        _inventory       = GetComponent<Inventory>();

        // Inicialización de la máquina de estados y sus estados
        StateMachine   = new PlayerStateMachine();
        IdleState      = new IdleState(StateMachine, this);
        JumpState      = new JumpState(StateMachine, this);
        WallSlideState = new WallSlideState(StateMachine, this);
        FallingState   = new FallingState(StateMachine, this);
        DashState      = new DashState(StateMachine, this);
        AttackState    = new AttackState(StateMachine, this);
        SprintState    = new SprintState(StateMachine, this);
        AttackRunState = new AttackRunState(StateMachine, this);
    }

    private void OnEnable()
    {
        if (_playerInputActions != null)
            _playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        if (_playerInputActions != null)
            _playerInputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        if (_playerInputActions != null)
            _playerInputActions.Dispose();
    }

    /// <summary>Inyección de dependencia de Zenject para el AudioManager.</summary>
    [Inject]
    public void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);

        if (_deathScreen != null)
            _deathScreen.gameObject.SetActive(false);

        if (uiOptions != null)
            uiOptions.SetActive(false);

    }

    private void Update()
    {
        if (_isDead) return;

        // =========================
        // INPUTS con PlayerInputActions
        // =========================
        jumpPressed   = _playerInputActions.Player.Jump.WasPressedThisFrame();
        jumpReleased  = _playerInputActions.Player.Jump.WasReleasedThisFrame();
        dashPressed   = _playerInputActions.Player.Dash.WasPressedThisFrame();
        attackPressed = _playerInputActions.Player.Attack.WasPressedThisFrame();
        _moveInput    = _playerInputActions.Player.Move.ReadValue<Vector2>().x;

        // =========================
        // INVENTORY manual (sin PlayerInputActions)
        // =========================
        inventoryPressed =
            (Keyboard.current?.bKey?.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.dpad.up.wasPressedThisFrame ?? false);

        // =========================
        // SHIFT manual solo si lo necesitas para sprint u otra lógica
        // =========================
        shiftPressedThisFrame =
            (Keyboard.current?.leftShiftKey?.wasPressedThisFrame ?? false) ||
            (Keyboard.current?.rightShiftKey?.wasPressedThisFrame ?? false);

        shiftHeld =
            (Keyboard.current?.leftShiftKey?.isPressed ?? false) ||
            (Keyboard.current?.rightShiftKey?.isPressed ?? false);

        if (_moveInput != 0)
            _lastInput = _moveInput;

        // =========================
        // PAUSA
        // =========================
        bool pausePressed =
            (Keyboard.current?.escapeKey?.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.startButton.wasPressedThisFrame ?? false);

        if (pausePressed)
            TogglePauseMenu();

        // =========================
        // INVENTARIO
        // =========================
        if (inventoryPressed)
            ToggleInventoryMenu();

        GroundCheck();
        WallCheck();
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        if (_isDead) return;

        if (_isPaused || _isInventoryOpen) return;

        if (_isDashing || _isAttackSliding)
        {
            StateMachine.CurrentState.PhysicsUpdate();
            return;
        }

        if (_moveInput != 0 && canMove)
        {
            _rigidbody2D.linearVelocity = new Vector2(_moveInput * speed, _rigidbody2D.linearVelocity.y);
        }
        else if (!_isGrounded)
        {
            // En el aire sin input: mantener inercia
        }
        else
        {
            _rigidbody2D.linearVelocity = new Vector2(0f, _rigidbody2D.linearVelocity.y);
        }

        StateMachine.CurrentState.PhysicsUpdate();
    }

    // =========================================================================
    // CHECKS DE COLISIÓN
    // =========================================================================

    private void GroundCheck()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, groundCheckRadius, groundLayer);

        if (_isGrounded)
        {
            _jumpCutting = false;
            _canDashAir  = true;
        }
    }

    private void WallCheck()
    {
        _isOnLeftWall  = Physics2D.OverlapCircle(_wallCheckLeft.position, wallCheckRadius, wallLayer);
        _isOnRightWall = Physics2D.OverlapCircle(_wallCheckRight.position, wallCheckRadius, wallLayer);
        _isOnWall      = (_isOnLeftWall || _isOnRightWall) && !_isGrounded;
    }

    // =========================================================================
    // MOVIMIENTO
    // =========================================================================

    public void Gravity(float gravity)
    {
        _rigidbody2D.gravityScale = gravity;
    }

    public void WallJump()
    {
        _wallJumping = true;
        _wallSliding = false;
        _isOnWall    = false;

        float jumpDirection = _isOnLeftWall ? 1f : -1f;

        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.linearVelocity = new Vector2(jumpDirection * wallJumpForceX, wallJumpForceY);

        StartCoroutine(WallJumpLock());
    }

    private IEnumerator WallJumpLock()
    {
        _wallJumping = true;
        _rigidbody2D.gravityScale = 7.5f;

        float elapsed = 0f;
        while (elapsed < _wallJumpLockTime)
        {
            _moveInput = 0f;
            elapsed += Time.deltaTime;
            yield return null;
        }

        _wallJumping = false;
    }

    // =========================================================================
    // PAUSA
    // =========================================================================

    private void TogglePauseMenu()
    {
        // Si el inventario está abierto, primero lo cerramos
        if (_isInventoryOpen)
            ToggleInventoryMenu();

        _isPaused = !_isPaused;

        if (uiOptions != null)
        {
            uiOptions.SetActive(_isPaused);

            if (_isPaused)
            {
                _playerInputActions.Player.Disable();

                OptionsMenuManager options = uiOptions.GetComponent<OptionsMenuManager>();
                if (options != null)
                    options.SetInitialFocus();
            }
            else
            {
                _playerInputActions.Player.Enable();
            }
        }
    }

    public void UnPauseMenu()
    {
        _isPaused = false;

        if (uiOptions != null)
            uiOptions.SetActive(false);

        if (_playerInputActions != null)
            _playerInputActions.Player.Enable();
    }

    // =========================================================================
    // INVENTARIO
    // =========================================================================

    private void ToggleInventoryMenu()
    {

        // Si el pause está abierto, no abrir inventario
        if (_isPaused) return;

        _isInventoryOpen = !_isInventoryOpen;

        if (_isInventoryOpen)
        {
            _playerInputActions.Player.Disable();
        }
        else
        {
            _playerInputActions.Player.Enable();
        }
    }

    // =========================================================================
    // FREEZE / UNFREEZE
    // =========================================================================

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

    public void TakeDamage()
    {
        StartCoroutine(HitEffect());
    }

    private IEnumerator HitEffect()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        _audioManager.PlaySFX(damage, sfxSource, 1f);

        if (sr != null) sr.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        if (sr != null) sr.color = Color.white;
    }

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

    public void Heal(float amount)
    {
        _audioManager.PlaySFX(heal, itemSource, 1f);
        Debug.Log($"Healing {amount}");
    }

    // =========================================================================
    // SAVE SYSTEM
    // =========================================================================

    public void LoadSavedGame()
    {
        OnLoadGame?.Invoke();
    }

    // =========================================================================
    // GIZMOS
    // =========================================================================

    private void OnDrawGizmosSelected()
    {
        if (_groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundCheck.position, groundCheckRadius);

        if (_wallCheckLeft != null)
            Gizmos.DrawWireSphere(_wallCheckLeft.position, wallCheckRadius);

        if (_wallCheckRight != null)
            Gizmos.DrawWireSphere(_wallCheckRight.position, wallCheckRadius);

        if (attackPoint != null && stats != null)
            Gizmos.DrawWireCube(attackPoint.position, stats.attackRange);
    }
}