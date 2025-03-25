using UnityEngine;

public abstract class Enemy : Entity 
{
    
    public MonsterInfo stat;

    public Animator enemyAnim;

    protected EnemyStateMachine stateMachine;

    virtual public void Awake()
    {
        stateMachine = new EnemyStateMachine();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    virtual public void Start()
    {
        enemyAnim = GetComponent<Animator>();
        stat = GameManager.Instance.EnemyStatInitialize(id);

        Initialize(stat.id,
            stat.characterName,
            stat.hp,
            stat.attackType,
            stat.attackRange,
            stat.atk,
            stat.attackSpeed,
            stat.moveSpeed,
            stat.maxJumpHeight,
            stat.minJumpHeight,
            stat.timeToJumpApex
            );
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    public abstract void Think();
    public abstract void Move();

    // 상태 변경 메서드
    public void ChangeToMoveState()
    {
        stateMachine.ChangeState(new MoveState(this));
    }

    public void ChangeToIdleState()
    {
        stateMachine.ChangeState(new IdleState(this));
    }
}
