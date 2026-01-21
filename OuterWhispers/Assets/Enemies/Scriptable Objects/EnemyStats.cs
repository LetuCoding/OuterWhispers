using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public float speed = 3f;
    public float detectionDistance = 5f;
    public float patrolWaitTime = 1f;
    public float maxHealth = 100f;
}
