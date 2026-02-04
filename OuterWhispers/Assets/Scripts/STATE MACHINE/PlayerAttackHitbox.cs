using UnityEngine;
using Core.Interfaces;

public class PlayerAttackHitbox : MonoBehaviour
{
    // Referencia al Player para saber cuánto daño hacemos
    [SerializeField] private Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Buscamos si lo que golpeamos tiene la interfaz de daño
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            // 2. Aplicamos el daño usando las stats del player
            damageable.TakeDamage(player.stats.attackDamage);
            Debug.Log("¡Golpeado: " + collision.name + "!");
        }
    }
}