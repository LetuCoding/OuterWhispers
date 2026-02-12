using UnityEngine;

public class EnemyHeavyAttackState : EnemyState
{
    private float timer;
    private bool hasPerformedStrike;
    
    private float chargeDuration = 2f;   
    private float hitboxActiveTime = 0.3f; 
    private float recoveryTime = 0.9f;     
    
    private float damageMultiplier = 2.0f;
    private float rangeMultiplier = 1.6f;

    public EnemyHeavyAttackState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.StopMovement();
        timer = 0f;
        hasPerformedStrike = false;

        bool isPlayerRight = enemy.playerTransform.position.x > enemy.transform.position.x;
        if (isPlayerRight) enemy.animator.Play("Heavy_Attack_Right");
        else enemy.animator.Play("Heavy_Attack_Left");
        
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
        }
    }

    private void PerformStrike()
    {
        /*if (enemy.audioManager != null)
        {
            enemy.audioManager.PlaySFX(enemy.audioManager.chains);
        }*/

        bool isPlayerRight = enemy.playerTransform.position.x > enemy.transform.position.x;
        if (isPlayerRight) enemy.animator.Play("Heavy_Attack_Right_Execution");
        else enemy.animator.Play("Heavy_Attack_Left_Execution");

        if (enemy.meleeHitbox != null)
        {
            enemy.meleeHitbox.transform.localScale = new Vector3(
                enemy.stats.attackRange.x * rangeMultiplier, 
                enemy.stats.attackRange.y * rangeMultiplier, 
                0
            );
            
            enemy.meleeHitbox.SetActive(true);
            
            var hbScript = enemy.meleeHitbox.GetComponent<EnemyAttackHitbox>();
            if (hbScript) hbScript.PrepareForAttack(damageMultiplier);
        }
        
        enemy.ResetColor();
    }
}