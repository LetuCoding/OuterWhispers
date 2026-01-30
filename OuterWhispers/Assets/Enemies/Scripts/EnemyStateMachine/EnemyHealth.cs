using System;
using Core.Interfaces;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyStats stats;
    private Enemy enemy;

    private float currentHealth;
    private float lastDamageTime;
    [SerializeField] private float damageCooldown = 0.2f;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = stats.maxHealth;
        lastDamageTime = -damageCooldown;
        enemy = GetComponent<Enemy>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead || Time.time - lastDamageTime < damageCooldown) return;

        lastDamageTime = Time.time;
        currentHealth -= damage;
        enemy.PlayHitFlash();
        Debug.Log("Vida actual enemigo:" + currentHealth);
        if (enemy.canBeStunned)
        {
            enemy.TriggerStun();
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (enemy != null)
        {
            enemy.TriggerDeath();
        }
        else
        {
            Destroy(gameObject);
        }

        // OPCIONAL: Si quieres que el cuerpo desaparezca a los 10 segundos
        // Destroy(gameObject, 10f); 
    }
}