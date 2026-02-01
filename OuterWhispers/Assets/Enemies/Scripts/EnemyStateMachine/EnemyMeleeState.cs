using Unity.VisualScripting;
using UnityEngine;

public class EnemyMeleeState : EnemyState
{
    private float timer;
    private bool isAttacking;
    
    private float attackDuration = 0.2f; 
    private float attackCooldown; 

    public EnemyMeleeState(EnemyStateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.StopMovement();
        
        PerformAttack();
        attackCooldown = enemy.stats.attackCooldown;
        isAttacking = true;
        timer = 0f;
    }

    public override void LogicUpdate()
    {
        timer += Time.deltaTime;

        if (isAttacking && timer >= attackDuration)
        {
            if (enemy.meleeHitbox != null) enemy.meleeHitbox.SetActive(false);
            isAttacking = false;
        }

        if (timer >= attackCooldown)
        {
            enemy.DecideNextCombatAction(); 
        }
    }

    public override void Exit()
    {
        if (enemy.meleeHitbox != null) enemy.meleeHitbox.SetActive(false);
    }

    private void PerformAttack()
    {
        bool isPlayerRight = enemy.playerTransform.position.x > enemy.transform.position.x;
        enemy.EnemyDirection = isPlayerRight;
        if (enemy.audioManager != null)
        {
                    
            enemy.audioManager.PlaySFX(enemy.audioManager.shoot);
        }
        
        if (isPlayerRight)
        {
            enemy.animator.Play("Attack_Right");
        }
        else
        {
            enemy.animator.Play("Attack_Left");
        }
        if (enemy.meleeHitbox != null)
        {
            enemy.meleeHitbox.transform.localScale = new Vector3(enemy.stats.attackRange.x, enemy.stats.attackRange.y, 0);
            enemy.meleeHitbox.SetActive(true);
            var hbScript = enemy.meleeHitbox.GetComponent<EnemyAttackHitbox>();
            if (hbScript) hbScript.PrepareForAttack();
        }
        
    }

}