using UnityEngine;
using System;
using Core.Interfaces;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStats stats;
    
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private float currentHealth;

    private void Start()
    {
        currentHealth = stats.maxHealth;
        OnHealthChanged?.Invoke(currentHealth, stats.maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        
        currentHealth = Mathf.Clamp(currentHealth, 0, stats.maxHealth);

        OnHealthChanged?.Invoke(currentHealth, stats.maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, stats.maxHealth);
        OnHealthChanged?.Invoke(currentHealth, stats.maxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} ha muerto.");
    }
}