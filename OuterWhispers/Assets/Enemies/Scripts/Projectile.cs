using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private Transform player;
    private Rigidbody2D rb;

    private void Start()
    {
        player = FindObjectOfType<Player>().transform;
        rb = GetComponent<Rigidbody2D>();
            LaunchProjectile();
    }

    private void LaunchProjectile()
    {
        Vector2 directionToTarget = (player.position - transform.position).normalized;
        rb.linearVelocity = directionToTarget * speed;
        
    }

    private void OnCollisionEnter2D()
    {
        Destroy(gameObject);
    }
}