using UnityEngine;
using System;
using System.Collections;
using Core.Interfaces;
using Interfaces;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour, IDamageable, IEffectTarget
{
    [SerializeField] private PlayerStats stats;
    
    public event Action<float, float> OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent OnDamage;

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
        
        //StartCoroutine(StopTimeOnDamage());
        
        OnDamage?.Invoke();
            
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

    public void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} ha muerto.");
    }
    
    public IEnumerator StopTimeOnDamage()
    {
        Time.timeScale = 0.25f;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        Debug.Log("StopTime.");
    }
    
}