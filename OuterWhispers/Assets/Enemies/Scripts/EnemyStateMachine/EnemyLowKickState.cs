using UnityEngine;

public class EnemyLowKickState : EnemyState
{
    private float timer;
    private bool hasPerformedStrike;
    private float chargeDuration = 0.5f;
    private float hitboxActiveTime = 0.3f; 
    private float recoveryTime = 0.7f;
    private float damageMultiplier = 1.5f;
    private float scaleXMultiplier = 2.2f; 
    private float scaleYMultiplier = 0.5f; 

    public EnemyLowKickState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.StopMovement();
        timer = 0f;
        hasPerformedStrike = false;
        
        bool isPlayerRight = enemy.playerTransform.position.x > enemy.transform.position.x;
        enemy.EnemyDirection = isPlayerRight;
        
        if (isPlayerRight) enemy.animator.Play("Smash_Attack_Right"); 
        else enemy.animator.Play("Smash_Attack_Left");
        
        if (enemy.meleeHitbox != null) enemy.meleeHitbox.SetActive(false);
    }

    public override void LogicUpdate()
    {
        timer += Time.deltaTime;
        
        if (timer >= chargeDuration && !hasPerformedStrike)
        {
            PerformStrike();
            hasPerformedStrike = true;
        }
        
        if (timer >= (chargeDuration + hitboxActiveTime))
        {
            if (enemy.meleeHitbox != null) enemy.meleeHitbox.SetActive(false);
        }

        if (timer >= (chargeDuration + hitboxActiveTime + recoveryTime))
        {
            float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);
            if (distanceToPlayer <= enemy.stats.detectionDistance)
                stateMachine.ChangeState(enemy.ChaseState);
            else
                stateMachine.ChangeState(enemy.PatrolState);
        }
    }

    public override void Exit()
    {
        enemy.ResetColor();

        if (enemy.meleeHitbox != null)
        {
            enemy.meleeHitbox.SetActive(false);
            enemy.meleeHitbox.transform.localScale = new Vector3(enemy.stats.attackRange.x, enemy.stats.attackRange.y, 0);
            enemy.meleeHitbox.transform.localPosition = new Vector3(enemy.meleeHitbox.transform.localPosition.x, 0f, 0f);
        }
    }

    private void PerformStrike()
    {
        if (AudioManagerEnemy.Instance != null)
            AudioManagerEnemy.Instance.PlaySFX(AudioManagerEnemy.Instance.smash);
        
        bool isPlayerRight = enemy.playerTransform.position.x > enemy.transform.position.x;
        if (isPlayerRight) enemy.animator.Play("Smash_Attack_Right_Execution");
        else enemy.animator.Play("Smash_Attack_Left_Execution");
        
        if (enemy.meleeHitbox != null)
        {
            float newWidth = enemy.stats.attackRange.x * scaleXMultiplier;
            enemy.meleeHitbox.transform.localScale = new Vector3(
                newWidth,
                enemy.stats.attackRange.y * scaleYMultiplier,
                0
            );
            
            float direction = isPlayerRight ? 1f : -1f;
            enemy.meleeHitbox.transform.localPosition = new Vector3(direction * (newWidth / 2), enemy.lowKickYOffset, 0);
            
            enemy.meleeHitbox.SetActive(true);
            var hbScript = enemy.meleeHitbox.GetComponent<EnemyAttackHitbox>();
            if (hbScript) hbScript.PrepareForAttack(damageMultiplier);
        }

        enemy.ResetColor();
    }
}