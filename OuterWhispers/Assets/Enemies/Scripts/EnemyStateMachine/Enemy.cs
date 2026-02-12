using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour, Core.Interfaces.IDamageable
{
    #region State Machine Variables
    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyMeleeState MeleeState { get; private set; }
    public EnemyShootState ShootState { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyStunState StunState { get; private set; }
    public EnemyDeathState DeathState { get; private set; }
    public EnemyHeavyAttackState HeavyAttackState { get; private set; }
    public EnemyLowKickState LowKickState { get; private set; }

    #endregion

    #region Components
    public EnemyStats stats;
    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    private EnemyHealth health;
    
    #endregion
    
    #region Audio
    public AudioSource audioSource;
    public AudioClip footstep;
    public AudioClip shoot;
    public AudioClip dead;
    public AudioClip chains;
    public AudioClip smash;
    public AudioClip damage;
    public float pitch = 1f;
    public IAudioManager _audioManager;
    #endregion

    #region Configuration & References
    [Header("Detection Settings")]
    public Transform playerTransform;
    
    [Header("Behavior Switches")]
    public bool canPatrol = true;
    public bool canChase = true;
    public bool canShoot = false;

    [Header("Patrol Settings")]
    public Transform[] waypoints;

    [Header("Melee Settings")]
    public GameObject meleeHitbox;
    public float meleeAttackOffset = 0.5f;
    public float lowKickYOffset = -0.5f;
    
    [Header("Combat Settings")]
    public float stunDuration = 0.8f;
    public bool canBeStunned = true;
    
    [Header("Special Attacks Settings")]
    public bool canUseHeavyAttack = false; 
    public bool canUseLowKick = false;
    
    
    [Header("Shoot Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootOffset = 1f;
    #endregion
    
    [Header("Visual Feedback")]
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    public Color originalColor;
    
    [Inject]
    public void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }
    #region State Variables
    public bool hasDetectedPlayer;
    public bool EnemyDirection;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        StateMachine = new EnemyStateMachine();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        PatrolState = new EnemyPatrolState(StateMachine, this);
        PatrolState = new EnemyPatrolState(StateMachine, this);
        PatrolState = new EnemyPatrolState(StateMachine, this);
        PatrolState = new EnemyPatrolState(StateMachine, this);
        ChaseState = new EnemyChaseState(StateMachine, this);
        MeleeState = new EnemyMeleeState(StateMachine, this);
        ShootState = new EnemyShootState(StateMachine, this);
        IdleState = new EnemyIdleState(StateMachine, this);
        StunState = new EnemyStunState(StateMachine, this);
        DeathState = new EnemyDeathState(StateMachine, this);
        HeavyAttackState = new EnemyHeavyAttackState(StateMachine, this);
        LowKickState = new EnemyLowKickState(StateMachine, this);
    }

    private void Start()
    {

        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        if (canPatrol)
            StateMachine.Initialize(PatrolState);
        else
            StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    #region Movement Methods

    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero; 
    }

    #endregion

    #region Combat Methods
    
    public void TriggerDeath()
    {
        StateMachine.ChangeState(DeathState);
    }
    
    public void DecideNextCombatAction()
    {
        if (playerTransform == null)
        {
            StateMachine.ChangeState(PatrolState);
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);
   
        if (distance > stats.attackRange.x)
        {
            StateMachine.ChangeState(ChaseState);
            return;
        }
        
        float chance = Random.value;

        if (canUseHeavyAttack && HeavyAttackState != null && chance <= 0.4f)
        {
            Debug.Log("Ataque Pesado");
            StateMachine.ChangeState(HeavyAttackState);
        }
        else if (canUseLowKick && LowKickState != null && chance > 0.4f && chance <= 0.75f)
        {
            Debug.Log("Patada Baja");
            StateMachine.ChangeState(LowKickState);
        }
        else
        {
            Debug.Log("Ataque Melee");
            StateMachine.ChangeState(MeleeState);
        }
    }
    
    public void DisablePhysics()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static; 
        }
        if (meleeHitbox != null) meleeHitbox.SetActive(false);
    }
    
    public void SetColor(Color color)
    {
        if (spriteRenderer != null) spriteRenderer.color = color;
    }

    public void ResetColor()
    {
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
    }
    
    public void TriggerStun()
    {
        StateMachine.ChangeState(StunState);
    }
    
    public void TakeDamage(float damage)
    {
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning("Enemy recibió daño pero no tiene componente EnemyHealth");
        }
    }
    public void PlayHitFlash()
    {
        if (spriteRenderer != null)
            StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (stats != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stats.detectionDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(stats.attackRange.x, stats.attackRange.y, 0));
        }
    }
    #endregion
}

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }
    public override void LogicUpdate() { enemy.StopMovement(); }
}