using UnityEngine;

public abstract class Enemy : Entity 
{
    
    public MonsterInfo stat;

    public Animator enemyAnim;

    protected EnemyStateMachine stateMachine;

    [Header("MeleeAttack")]
    public bool isAttack;
    public Vector2 meleeBoxSize;
    protected Vector2 meleeBoxPosition;
    protected float attackDir = 1;
    protected bool enableAttackBox = false;

    public NormalRoom curNormalRoom;


    virtual public void Awake()
    {
        stateMachine = new EnemyStateMachine();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    virtual public void Start()
    {
        enemyAnim = GetComponent<Animator>();
        stat = GameManager.Instance.EnemyStatInitialize(id);

        InitializeEnemy();
    }

    private void OnEnable()
    {
        stat = GameManager.Instance.EnemyStatInitialize(id);

        InitializeEnemy();
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    public abstract void Think();
    public abstract void Move();
    public abstract void AttackStart();
    public abstract void AttackEnd();


    // 상태 변경 메서드
    protected void ChangeToMoveState()
    {
        stateMachine.ChangeState(new MoveState(this));
    }

    protected void ChangeToIdleState()
    {
        stateMachine.ChangeState(new IdleState(this));
    }

    protected void ChangeToAttackState()
    {
        stateMachine.ChangeState(new AttackState(this));

    }
    protected void ChangeToDeadState()
    {
        stateMachine.ChangeState(new DeadState(this));

    }


    public override void EntityDeadCheck()
    {
        if (curHP <= 0)
        {
            curHP = 0;
            EnemyPoolManager.Instance.ReturnEnemy(gameObject, stat.characterName);

            // 카운트로 바꾸기

            curNormalRoom.enemyCnt--;
            ChangeToDeadState();

            gameObject.SetActive(false);
        }
    }

    public void InitializeEnemy()
    {
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

        ChangeToIdleState();
    }
    public void InitializeEnemy(NormalRoom room)
    {
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

        curNormalRoom = room;
        ChangeToIdleState();

    }
}
