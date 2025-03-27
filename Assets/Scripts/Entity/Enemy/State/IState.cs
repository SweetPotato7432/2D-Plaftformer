using UnityEngine;

public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public class IdleState : IState
{
    private Enemy enemy;

    public IdleState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter() 
    {
        enemy.state = Entity.States.IDLE;
        // 일정 시간이 지나면 이동 상태로 전환
        enemy.Invoke("ChangeToMoveState", 2f);
    }
    public void Update()
    {
        
    }
    public void Exit() { }
}

public class MoveState : IState
{
    private Enemy enemy;

    public MoveState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter() 
    {
        enemy.state = Entity.States.MOVE;
        enemy.enemyAnim.SetBool("isMove", true);

        enemy.Think();
    }
    public void Update() 
    {
        enemy.Move();
    }
    public void Exit()
    {
        enemy.enemyAnim.SetBool("isMove", false);
        enemy.CancelInvoke();
    }
}

public class AttackState : IState
{
    private Enemy enemy;

    public AttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.state = Entity.States.ATTACK;
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}
