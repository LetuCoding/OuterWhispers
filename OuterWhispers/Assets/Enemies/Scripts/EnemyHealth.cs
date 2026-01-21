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