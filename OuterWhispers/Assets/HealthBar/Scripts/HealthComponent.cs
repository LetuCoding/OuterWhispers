using UnityEngine;
using System;
using System.Collections;
using Core.Interfaces;
using Interfaces;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour, IDamageable, IEffectTarget
{
    
    private PlayerStats _stats;
    
    public event Action<float, float> OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent OnDamage;
    public UnityEvent OnHeal;
    private float currentHealth;

    public float CurrentHealth => currentHealth;
    private void Start()
    {
        _stats = GetComponent<Player>().stats;;
        currentHealth = _stats.maxHealth;
        OnHealthChanged?.Invoke(currentHealth, _stats.maxHealth);
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
    }
    
    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        
        currentHealth = Mathf.Clamp(currentHealth, 0, _stats.maxHealth);

        OnHealthChanged?.Invoke(currentHealth, _stats.maxHealth);
        
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
        currentHealth = Mathf.Clamp(currentHealth, 0, _stats.maxHealth);
        OnHealthChanged?.Invoke(currentHealth, _stats.maxHealth);
        OnHeal?.Invoke();
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