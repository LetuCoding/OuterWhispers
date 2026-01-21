using UnityEngine;
using System.Collections;

public class ShootBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootZone;
    [SerializeField] private float timeBetweenShots = 1f;
    [SerializeField] private float shootOffset = 1f;

    private Transform player;
    private Coroutine shootCoroutine;
    
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void StartShooting()
    {
        if (shootCoroutine == null)
            shootCoroutine = StartCoroutine(ShootRoutine());
    }

    public void StopShooting()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            UpdateShootZonePosition();
            Shoot();
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }
        
    private void UpdateShootZonePosition()
    {
        if (player == null || shootZone == null) return;

        float direction = (player.position.x > transform.position.x) ? 1f : -1f;
        
        shootZone.localPosition = new Vector3(direction * shootOffset, shootZone.localPosition.y, 0);
        
        shootZone.right = (player.position - shootZone.position).normalized;
    }

    private void Shoot()
    {
        if (player == null || projectilePrefab == null || shootZone == null)
            return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            shootZone.position,
            shootZone.rotation
        );

        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
            proj.Initialize(player);
    }
}