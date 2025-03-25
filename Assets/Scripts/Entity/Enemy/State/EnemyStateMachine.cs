public class EnemyStateMachine
{
    private IState currentState;

    public void ChangeState(IState newState)
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
