using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public float speed = 3f;
    public float detectionDistance = 5f;
    public float patrolWaitTime = 1f;
    public float maxHealth = 100f;
    
    public Vector2 attackRange = new Vector2(1.1f, 0.7f);
    public float attackDamage = 20f;
    public float attackCooldown = 0.8f;
}
