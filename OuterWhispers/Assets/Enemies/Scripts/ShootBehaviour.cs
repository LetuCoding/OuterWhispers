using UnityEngine;
using System.Collections;

public class ShootBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootZone;
    [SerializeField] private float timeBetweenShots = 1f;
    public Animator _animator;

    private Transform player;
    private Coroutine shootCoroutine;
    
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void StartShooting()
    {
        AudioManagerEnemy.Instance.StopWalk();
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
            if (AudioManagerEnemy.Instance != null)
                AudioManagerEnemy.Instance.PlaySFX(AudioManagerEnemy.Instance.shoot);
            _animator.Play("Attack_Left");
            Shoot();
            yield return new WaitForSeconds(timeBetweenShots);
        }
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