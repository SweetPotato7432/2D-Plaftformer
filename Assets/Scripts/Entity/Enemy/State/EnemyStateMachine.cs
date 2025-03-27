public class EnemyStateMachine
{
    private IEnemyState currentState;

    public void ChangeState(IEnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // Update is called once per frame
    public void Update()
    {
        currentState?.Update();
    }
}
