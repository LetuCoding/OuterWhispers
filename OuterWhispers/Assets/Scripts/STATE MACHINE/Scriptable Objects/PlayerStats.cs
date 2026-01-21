using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public float maxHealth = 100f;
    public float attackDamage = 20f;
    public float attackCooldown = 0.1f;
    public Vector2 attackRange = new Vector2(1.5f, 0.7f);
}