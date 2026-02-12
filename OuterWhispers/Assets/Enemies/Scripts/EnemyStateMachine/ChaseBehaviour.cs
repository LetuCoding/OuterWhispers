using Enemies.Interfaces;
using UnityEngine;

public class ChaseBehaviour : MonoBehaviour, IEnemyBehaviour
{
    
    [SerializeField] private EnemyStats stats;
    [SerializeField] private Transform detectionArea;
    [SerializeField] private MeleeBehaviour meleeBehaviour;
    [SerializeField] private Animator _animator;
    
    private Transform player;
    private Transform enemyTransform;
    private bool isInRange = false;
    private bool isWalking = false;

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
        if (meleeBehaviour != null) meleeBehaviour.SetPlayer(player);
    }

    public void Enter()
    {
        enemyTransform = transform;

    }

    public void Execute()
    {
        if (player == null) return;
        if (isWalking == false)
        {
            // enemy.audioManager.PlayWalk();
            isWalking = true;
        }
        
        float distanceToPlayer = Vector2.Distance(enemyTransform.position, player.position);

        if (distanceToPlayer <= stats.attackRange.x)
        {
            if (!isInRange)
            {
                isInRange = true;
                if (meleeBehaviour != null) meleeBehaviour.StartAttacking();
            }
        }
        else
        {
            if (isInRange)
            {
                isInRange = false;
                if (meleeBehaviour != null) meleeBehaviour.StopAttacking();
            }
            
            enemyTransform.position = new Vector3(Vector2.MoveTowards(
                enemyTransform.position,
                player.position,
                stats.speed * Time.deltaTime
            ).x, enemyTransform.position.y, 0);
            UpdateWalkAnimation();
        }

        UpdateFacingDirection();
    }
    
    private void UpdateWalkAnimation()
    {
        if (_animator == null) return;

        if (player.position.x > enemyTransform.position.x)
        {
            _animator.Play("Walk_Right");
        }
        else
        {
            _animator.Play("Walk_Left");
        }
    }

    private void UpdateFacingDirection()
    {
        bool shouldFaceRight = player.position.x > enemyTransform.position.x;
        if (meleeBehaviour != null)
        {
            meleeBehaviour.UpdateHitboxSide(shouldFaceRight);
        }
    }

    public void Exit()
    {
        if (isInRange)
        {
            if (meleeBehaviour != null) meleeBehaviour.StopAttacking();
            isInRange = false;
        }
        //if (isWalking == true)
            // enemy.audioManager.StopWalk();
    }
    
    private void OnDrawGizmosSelected()
    {
        if (detectionArea != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectionArea.position, stats.detectionDistance);
        }

    }
}