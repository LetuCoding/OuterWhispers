using UnityEngine;

[RequireComponent(typeof(EnemyStateMachine))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyStateMachine stateMachine;
    [SerializeField] private PatrolBehaviour patrolBehaviour;
    [SerializeField] private ChaseBehaviour chaseBehaviour;
    [SerializeField] private ShootBehaviour shootBehaviour;
    [SerializeField] private EnemyStats stats;
    [SerializeField] private Transform player;
    
    [SerializeField] private bool canPatrol = true;
    [SerializeField] private bool canChase = true;
    [SerializeField] private bool canShoot = false;

    private bool hasDetectedPlayer = false;

    private void Start()
    {
        //if (canPatrol && patrolBehaviour != null)
            //stateMachine.ChangeState(patrolBehaviour);
    }

    private void Update()
    {
        if (player == null || hasDetectedPlayer)
            return;

        if (Vector3.Distance(transform.position, player.position) <= stats.detectionDistance)
        {
            hasDetectedPlayer = true;

            if (canChase && chaseBehaviour != null)
            {
                chaseBehaviour.SetPlayer(player);
                //stateMachine.ChangeState(chaseBehaviour);
            }

            if (canShoot && shootBehaviour != null)
            {
                stateMachine.ChangeState(null);
                shootBehaviour.SetPlayer(player);
                shootBehaviour.StartShooting();
            }
        }
    }
}