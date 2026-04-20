using UnityEngine;
using Core.Interfaces; // IDamageable debe estar definido aquí; ajusta el namespace si difiere

/// <summary>
/// Hitbox de ataque del jugador. Se activa/desactiva desde los estados de ataque
/// (<see cref="AttackState"/>, <see cref="AttackRunState"/>) a través del
/// <c>attackPoint</c> del jugador.
///
/// Cuando un collider entra en el trigger, busca <see cref="IDamageable"/> en
/// el objeto golpeado y aplica el daño configurado en <see cref="PlayerStats"/>.
/// </summary>
public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] private Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(player.stats.attackDamage);
            Debug.Log($"Hit: {collision.name} for {player.stats.attackDamage} damage.");
        }
    }
}