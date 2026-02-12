using System;
using UnityEngine;
using System.Collections;
using Zenject;

public class MeleeBehaviour : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;
    [SerializeField] private GameObject hitboxObject;
    [SerializeField] private float attackOffset = 0.5f;

    private bool shouldFaceRight;
    private bool IsHitboxOnRight;
    private Animator _animator;
    private Transform player;
    private Coroutine attackCoroutine;
    private Enemy _enemy;

    [Inject]
    public void Construct(Enemy enemy)
    {
        this._enemy = enemy;
    }
    public void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void SetHitbox()
    {
        hitboxObject.transform.localScale = new Vector3(stats.attackRange.x, stats.attackRange.y, 1);
        
        hitboxObject.transform.localPosition = new Vector3(attackOffset, 0, 0);;
    }

    public void StartAttacking()
    {
        _enemy._audioManager.StopWalk(_enemy.audioSource);
        if (attackCoroutine == null)
            if (IsHitboxOnRight == false)
            {
                _animator.Play("Attack_Left_Final");
            }
            else
            {
                _animator.Play("Attack_Right_Final");
            }
            _enemy._audioManager.PlaySFX(_enemy.shoot,_enemy.audioSource,_enemy.pitch);
            attackCoroutine = StartCoroutine(AttackRoutine());
    }

    public void StopAttacking()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        if (hitboxObject != null) hitboxObject.SetActive(false);
    }

    private IEnumerator AttackRoutine()
    {
        SetHitbox();
 
        EnemyAttackHitbox hitboxScript = hitboxObject.GetComponent<EnemyAttackHitbox>();
        
        while (true)
        {
            if (player != null && player.TryGetComponent(out Rigidbody2D rb)) {
                rb.WakeUp(); 
            }

            if (hitboxScript != null) hitboxScript.PrepareForAttack();
        
            hitboxObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            if (IsHitboxOnRight == false)
            {
                _animator.Play("Attack_Left");
            }
            else
            {
                _animator.Play("Attack_Right");
            } 
            hitboxObject.SetActive(false);

            yield return new WaitForSeconds(stats.attackCooldown);
            if (IsHitboxOnRight == false)
            {
                _animator.Play("Attack_Left_Final");
            }
            else
            {
                _animator.Play("Attack_Right_Final");
            }
            _enemy._audioManager.PlaySFX(_enemy.shoot,_enemy.audioSource,_enemy.pitch);
        }
    }
    
    public void UpdateHitboxSide(bool faceRight)
    {
        if (hitboxObject == null) return;
        IsHitboxOnRight = faceRight;
        float direction = faceRight ? 1f : -1f;
        hitboxObject.transform.localPosition = new Vector3(direction * attackOffset, 0, 0);
    }
    
    private void OnDrawGizmosSelected()
    {
        
        if (stats != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(hitboxObject.transform.position, hitboxObject.transform.lossyScale);
        }
    }
}