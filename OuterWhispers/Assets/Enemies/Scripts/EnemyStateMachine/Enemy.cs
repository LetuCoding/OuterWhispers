using UnityEngine;

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
    #endregion

    #region Components
    public EnemyStats stats;
    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    private EnemyHealth health;
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
    
    [Header("Combat Settings")]
    public float stunDuration = 0.8f;
    
    [Header("Shoot Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootOffset = 1f;
    #endregion
    
    [Header("Visual Feedback")]
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;

    #region State Variables
    public bool hasDetectedPlayer;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        StateMachine = new EnemyStateMachine();
        
        PatrolState = new EnemyPatrolState(StateMachine, this);
        ChaseState = new EnemyChaseState(StateMachine, this);
        MeleeState = new EnemyMeleeState(StateMachine, this);
        ShootState = new EnemyShootState(StateMachine, this);
        IdleState = new EnemyIdleState(StateMachine, this);
        StunState = new EnemyStunState(StateMachine, this);
        DeathState = new EnemyDeathState(StateMachine, this);
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
        Color originalColor = Color.white;
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