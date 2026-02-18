using System.Collections;
using _Project.Scripts.Gameplay.PlayerScripts.STATE_MACHINE;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class Player : MonoBehaviour, IEffectTarget, IPlayer
{
    //=====================================================================================================
    // COMPONENTS & INPUT
    //=====================================================================================================

    public PlayerInputActions _playerInputActions;
    public Rigidbody2D _rigidbody2D;
    public Animator _animator;
    public IAudioManager _audioManager;
    public HealthComponent _healthComponent;
    public float _moveInput;
    public float _lastInput;
    public bool _isGrounded;
    public bool _jumpCutting;
    public bool _canDashAir;
    public bool _isDashing;

    public bool jumpPressed;
    public bool jumpReleased;
    public bool dashPressed;


    //WallState
    public bool _isOnWall;
    public bool _isOnLeftWall;
    public bool _isOnRightWall;
    public bool _wallJumping;
    public bool _wallSliding;

    [Header("Attack Settings")] 
    public PlayerStats stats;
    public Transform attackPoint;
    public LayerMask enemyLayer;

    public AttackState AttackState { get; private set; }

    public bool attackPressed;
    //=====================================================================================================
    // GROUND CHECK SETTINGS
    //=====================================================================================================

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    //=====================================================================================================
    // AUDIO SETTINGS
    //=====================================================================================================
    [Header("Audio Sources")]
    [SerializeField] public AudioSource sfxSource;

    [Header("SFX Clips")]
    public AudioClip footstep;
    public AudioClip dash;
    public AudioClip jump;
    public AudioClip slide;
    public AudioClip punch;
    public AudioClip die;
    public AudioClip damage;
    
    //=====================================================================================================
    // WALL CHECK SETTINGS
    //=====================================================================================================

    [SerializeField] private Transform _wallCheckLeft;
    [SerializeField] private Transform _wallCheckRight;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckRadius = 0.1f;

    [SerializeField] internal float wallSlideGravity { get; } = 1f;
    [SerializeField] private float wallJumpForceX;
    [SerializeField] private float wallJumpForceY;
    private float _wallJumpLockTime = .125f;


    //======================================================================>
    //  STATE MACHINE AND STATES
    //======================================================================>

    #region StateMachine
    
    public PlayerStateMachine StateMachine { get; private set; }

    [Inject]
    public void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }
    public IdleState IdleState { get; private set; }
    public JumpState JumpState { get; private set; }

    public FallingState FallingState { get; private set; }

    public WallSlideState WallSlideState { get; private set; }

    public DashState DashState { get; private set; }
   

    private bool _isDead;
    

    #endregion
   

    //=====================================================================================================
    // PUBLIC CONFIGURATION
    //=====================================================================================================

    [Header("Movement Settings")] public float speed;
    public float jumpForce;

    [Header("Dash Settings")] [SerializeField]
    public float _dashDuration;

    [SerializeField] public float _dashSpeed;

    [Header("UI")]
    [SerializeField] private GameObject uiOptions;
    private bool _isPaused;
    
    void Awake()
    {

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _healthComponent =  GetComponent<HealthComponent>();
        //States inicializados
        StateMachine = new PlayerStateMachine();
        IdleState = new IdleState(StateMachine, this);
        JumpState = new JumpState(StateMachine, this);
        WallSlideState = new WallSlideState(StateMachine, this);
        FallingState = new FallingState(StateMachine, this);
        DashState = new DashState(StateMachine, this);
        AttackState = new AttackState(StateMachine, this);
    }



    //Al iniciar la máquina de estados le damos "Idle" por defecto para que tenga algo con lo que trabajar
    void Start()
    {
        StateMachine.Initialize(IdleState);

    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead) return;
        //Suscribimos ciertos checks al inputActions del jugador
        jumpPressed = _playerInputActions.Player.Jump.WasPressedThisFrame();
        jumpReleased = _playerInputActions.Player.Jump.WasReleasedThisFrame();
        dashPressed = _playerInputActions.Player.Dash.WasPressedThisFrame();
        _moveInput = _playerInputActions.Player.Move.ReadValue<Vector2>().x;

        if (_moveInput != 0)
        {
            _lastInput = _moveInput;
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }
        GroundCheck();
        WallCheck();
        StateMachine.CurrentState.LogicUpdate();
    }

    void FixedUpdate()
    {
        if (_isDead) return;

        // Para evitar conflictos de físicas, si estamos dasheando no aplicamos movimiento y volvemos.
        if (_isDashing)
        {
            StateMachine.CurrentState.PhysicsUpdate();
            return;
        }



        if (_moveInput != 0)
        {
            // Si hay input, el jugador tiene control total
            _rigidbody2D.linearVelocity = new Vector2(_moveInput * speed, _rigidbody2D.linearVelocity.y);
        }
        else
        {
            // SI NO HAY INPUT Y ESTÁ EN EL AIRE: No forzar a 0.
            if (!_isGrounded)
            {
                // No hacemos nada con la X, dejamos que siga como va.
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _rigidbody2D.linearVelocity.y);
            }
            else
            {
                // Si está en el suelo y no hay input, lo frenamos en seco
                _rigidbody2D.linearVelocity = new Vector2(0, _rigidbody2D.linearVelocity.y);
            }
        }




        StateMachine.CurrentState.PhysicsUpdate();
    }





    private void GroundCheck()
    {
        // Se detecta ÚNICAMENTE el suelo (gracias al LayerMask específico)
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, groundCheckRadius, groundLayer);

        // Reset de acciones permitidas al tocar suelo
        if (_isGrounded)
        {
            _jumpCutting = false;
            _canDashAir = true;
        }
    }

    //Método para cambiar la gravedad del jugador
    public void Gravity(float gravity)
    {
        _rigidbody2D.gravityScale = gravity;
    }


    //Método para el salto de pared del jugador
    public void WallJump()
    {
        _wallJumping = true;
        _wallSliding = false;
        _isOnWall = false;

        // Dirección REAL, se comrpueba en qué pared está para saber hacia dónde impulsarse.
        float jumpDirection = _isOnLeftWall ? 1 : -1;

        // Limpia velocidad en Y para salto consistente
        _rigidbody2D.linearVelocity = Vector2.zero;

        // Impulso del wall jump
        _rigidbody2D.linearVelocity = new Vector2(jumpDirection * wallJumpForceX, wallJumpForceY);

        StartCoroutine(WallJumpLock());
    }


    //Corutina para evitar que el jugador se pegue directamente a la pared al saltar (modificar según necesidad)
    private System.Collections.IEnumerator WallJumpLock()
    {
        _wallJumping = true;

        // Bloquea el input horizontal pero NO la física
        float lockTime = _wallJumpLockTime;
        float time = 0f;
        _rigidbody2D.gravityScale = 7.5f;
        while (time < lockTime)
        {

            _moveInput = 0; // evitar que el jugador cambie dirección
            time += Time.deltaTime;
            yield return null;
        }

        _wallJumping = false;
    }

    private void TogglePauseMenu()
    {
        _isPaused = !_isPaused;
        if (uiOptions != null)
            uiOptions.SetActive(_isPaused);
    }
    //Método que comprueba si el jugador está en alguna pared, izquierda o derecha.
    private void WallCheck()
    {
        _isOnLeftWall = Physics2D.OverlapCircle(_wallCheckLeft.position, wallCheckRadius, wallLayer);
        _isOnRightWall = Physics2D.OverlapCircle(_wallCheckRight.position, wallCheckRadius, wallLayer);

        _isOnWall = (_isOnLeftWall || _isOnRightWall) && !_isGrounded;


    }

    //Dibuja esferas en los objetos que se usan para detectar los checks anteriores, groundcheck, wallcheck... (se ven en el editor)
    private void OnDrawGizmosSelected()
    {
        if (_groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(_wallCheckLeft.position, wallCheckRadius);
        Gizmos.DrawWireSphere(_wallCheckRight.position, wallCheckRadius);

        Gizmos.DrawWireCube(attackPoint.position, stats.attackRange);
    }

    public void Heal(float amount)
    {
        Debug.Log("Healing " + amount);
    }

    public void TakeDamage()
    {

        StartCoroutine(HitEffect());
;
    }
    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        if (_lastInput == 1)
        {
            _animator.Play("Die_Right");
        }
        else
        {
            _animator.Play("Die_Left");
        }
        FreezePlayer();
        _audioManager.PlaySFX(die,sfxSource,1f);
    }
    private void FreezePlayer()
    {
        // Desactiva lectura de input
        if (_playerInputActions != null)
            _playerInputActions.Disable();

        // Para el movimiento
        if (_rigidbody2D != null)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
            _rigidbody2D.bodyType = RigidbodyType2D.Static; // opcional: congela total
        }
    }

    private IEnumerator HitEffect()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        _audioManager.PlaySFX(damage,sfxSource,1f);
        if (sr != null)
            sr.color = Color.red;
        
        yield return new WaitForSeconds(0.25f);

        if (sr != null)
            sr.color = Color.white;
    }

}
