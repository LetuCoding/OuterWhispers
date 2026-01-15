using Enemies.Interfaces;
using UnityEngine;

public class ChaseBehaviour : MonoBehaviour, IEnemyBehaviour
{
    [SerializeField] private EnemyStats stats;
    [SerializeField] private Transform detectionArea;
    private Transform player;
    private Transform enemyTransform;

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void Enter()
    {
        enemyTransform = transform;
    }

    public void Execute()
    {
        if (player == null)
            return;

        enemyTransform.position = new Vector3(Vector2.MoveTowards(
            enemyTransform.position,
            player.position,
            stats.speed * Time.deltaTime
        ).x, enemyTransform.position.y, 0);
        
        bool shouldFaceRight = player.position.x > enemyTransform.position.x;
        if ((shouldFaceRight && enemyTransform.localScale.x < 0) ||
            (!shouldFaceRight && enemyTransform.localScale.x > 0))
        {
            Vector3 scale = enemyTransform.localScale;
            scale.x *= -1;
            enemyTransform.localScale = scale;
        }
    }

    public void Exit()
    {
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionArea.position, stats.detectionDistance);
    }
}