using Enemies.Interfaces;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private IEnemyBehaviour currentState;

    public void ChangeState(IEnemyBehaviour newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        if (currentState != null)
            currentState.Enter();
    }

    private void Update()
    {
        currentState?.Execute();
    }
}