using UnityEngine;

public interface IEnemyState
{
    void Enter();
    void Update();
    void Exit();
}

public class IdleState : IEnemyState
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
        enemy.Invoke("ChangeToMoveState", 0.5f);
    }
    public void Update()
    {
        
    }
    public void Exit() { }
}

public class MoveState : IEnemyState
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

public class AttackState : IEnemyState
{
    private Enemy enemy;

    public AttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.enemyAnim.SetTrigger("isAttack");
        enemy.state = Entity.States.ATTACK;
        //enemy.AttackStart();
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}

public class DeadState : IEnemyState
{
    private Enemy enemy;

    public DeadState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.enemyAnim.SetTrigger("isDead");
        enemy.state = Entity.States.DEAD;
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}
