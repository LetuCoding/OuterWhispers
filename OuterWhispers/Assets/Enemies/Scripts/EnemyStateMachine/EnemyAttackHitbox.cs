using UnityEngine;
using Core.Interfaces;

public class EnemyAttackHitbox : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;
    private bool hasHit = false;
    private float currentDamageMultiplier = 2f;
    private BoxCollider2D boxCollider;
    
    public void PrepareForAttack(float damageMultiplier = 1f) 
    {
        hasHit = false;
        currentDamageMultiplier = damageMultiplier;
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hasHit || !collision.CompareTag("Player")) return;
        
        if (collision.TryGetComponent(out IDamageable damageableTarget))
        {
            float finalDamage = stats.attackDamage * currentDamageMultiplier;
            
            damageableTarget.TakeDamage(finalDamage);
            Debug.Log($"Golpe aplicado: {finalDamage} de daño");
            hasHit = true; 
        }
    }
    private void OnDrawGizmos()
    {
        if (!enabled && !gameObject.activeInHierarchy) return;
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f); 
            Gizmos.DrawCube(boxCollider.bounds.center, boxCollider.bounds.size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
        }
    }
}