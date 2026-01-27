using System;
using UnityEngine;
using System.Collections;

public class MeleeBehaviour : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;
    [SerializeField] private GameObject hitboxObject;
    [SerializeField] private float attackOffset = 0.5f;

    private bool shouldFaceRight;
    private Animator _animator;
    private Transform player;
    private Coroutine attackCoroutine;

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
        if (attackCoroutine == null)
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
            yield return new WaitForSeconds(0.1f);
            hitboxObject.SetActive(false);

            yield return new WaitForSeconds(stats.attackCooldown);
        }
    }
    
    public void UpdateHitboxSide(bool faceRight)
    {
        if (hitboxObject == null) return;
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