using UnityEngine;

public class EnemyShootState : EnemyState
{
    private float shootTimer;
    private bool isPlayerRight;

    public EnemyShootState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.StopMovement();
        shootTimer = 0.2f;
    }

    public override void LogicUpdate()
    {
        if (enemy.playerTransform == null) return;
        
        UpdateShootPosition();
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0)
        {
            Shoot();
            shootTimer = enemy.stats.attackCooldown > 0 ? enemy.stats.attackCooldown : 1f; 
        }
        
        float distance = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);
        if (distance > enemy.stats.detectionDistance * 1.5f)
        {
            stateMachine.ChangeState(enemy.PatrolState);
        }
    }

    private void UpdateShootPosition()
    {
        isPlayerRight = enemy.playerTransform.position.x > enemy.transform.position.x;
        float direction = isPlayerRight ? 1f : -1f;
        
        
        if (enemy.shootPoint != null)
        {
            enemy.shootPoint.localPosition = new Vector3(direction * enemy.shootOffset, enemy.shootPoint.localPosition.y, 0);
            
            Vector2 dirToPlayer = (enemy.playerTransform.position - enemy.shootPoint.position).normalized;
            enemy.shootPoint.right = dirToPlayer;
        }
        
    }

    private void Shoot()
    {
        if (enemy.projectilePrefab == null || enemy.shootPoint == null) return;
        
        /*if (enemy.audioManager != null)
        {
                    
            enemy.audioManager.PlaySFX(enemy.audioManager.shoot);
        }*/
        enemy.EnemyDirection = isPlayerRight;
        if (isPlayerRight)
        {
            enemy.animator.Play("Attack_Right");
        }
        else
        {
            enemy.animator.Play("Attack_Left");
        }
        
        GameObject proj = Object.Instantiate(enemy.projectilePrefab, enemy.shootPoint.position, enemy.shootPoint.rotation);
        
        Projectile pScript = proj.GetComponent<Projectile>();
        if (pScript != null) 
            pScript.Initialize(enemy.playerTransform);
    }
}