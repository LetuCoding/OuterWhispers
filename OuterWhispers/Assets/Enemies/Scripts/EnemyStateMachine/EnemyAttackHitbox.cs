using UnityEngine;
using Core.Interfaces;

public class EnemyAttackHitbox : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;
    private bool hasHit = false;

    public void PrepareForAttack() => hasHit = false;
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hasHit || !collision.CompareTag("Player")) return;
        
        if (collision.TryGetComponent(out IDamageable damageableTarget))
        {
            damageableTarget.TakeDamage(stats.attackDamage);
            Debug.Log("Golpe aplicado");
            hasHit = true; 
        }
    }
}
