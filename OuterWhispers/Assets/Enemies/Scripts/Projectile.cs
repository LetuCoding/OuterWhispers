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

    private void OnCollisionEnter2D()
    {
        Destroy(gameObject);
    }
}