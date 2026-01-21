using System;
using Core.Interfaces;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyStats stats;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = stats.maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            PlayerStats playerStats = collision.GetComponentInParent<Player>().stats;
            
            if (playerStats != null)
            {
                TakeDamage(playerStats.attackDamage);
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{name} recibió {damage} de daño. Vida restante: {currentHealth}");
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    
}