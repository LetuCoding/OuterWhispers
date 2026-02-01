using Core.Interfaces;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifeTime = 4f;

    private Transform target;
    private Rigidbody2D rb;

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (target != null)
            LaunchProjectile();
        
        Destroy(gameObject, lifeTime);
    }

    private void LaunchProjectile()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.TryGetComponent(out IDamageable damageableTarget))
        {
            damageableTarget.TakeDamage(10f); 
        }
        Destroy(gameObject);
    }
}