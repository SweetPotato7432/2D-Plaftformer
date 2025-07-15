using Microlight.MicroBar;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Enemy : Entity 
{
    [SerializeField]
    MicroBar hpBar;

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

    [Header("HPParticle")]
    //the HP Particle
    public GameObject HPParticle;

    //Default Forces
    Vector3 DefaultForce = new Vector3(0f, 200f, 0f);
    float DefaultForceScatter = 100f;

    bool isDead = false;

    virtual public void Awake()
    {
        stateMachine = new EnemyStateMachine();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    virtual public void Start()
    {
        if (hpBar != null) hpBar.Initialize(maxHP);
        hpBar.gameObject.SetActive(false);

        enemyAnim = GetComponent<Animator>();
        stat = GameManager.Instance.EnemyStatInitialize(id);

        InitializeEnemy();
    }

    private void OnEnable()
    {
        //stat = GameManager.Instance.EnemyStatInitialize(id);

        //InitializeEnemy();
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

    public override void TakeDamage(float damage)
    {
        if (!hpBar.gameObject.activeSelf)
        {
            hpBar.gameObject.SetActive(true);
        }
        base.TakeDamage(damage);
        if (hpBar != null) hpBar.UpdateBar(curHP, false, UpdateAnim.Damage);

        // 체력 시각화
        //GameObject NewHPP = Instantiate(HPParticle, this.transform.position, gameObject.transform.rotation) as GameObject;
        GameObject NewHPP = EffectPoolManager.Instance.GetEffect("Damage");
        NewHPP.GetComponent<HPParticleScript>().Initalize(this.transform.position, new Quaternion(0,180,0,0));
        //NewHPP.GetComponent<AlwaysFace>().Target = GameObject.Find("Main Camera").gameObject;
        NewHPP.SetActive(true);

        TextMesh TM = NewHPP.transform.Find("HPLabel").GetComponent<TextMesh>();

        if (damage > 0f)
        {
            TM.text = damage.ToString();
            TM.color = new Color(1f, 0f, 0f, 1f);
        }

        Vector2 force = new Vector2(DefaultForce.x + Random.Range(-DefaultForceScatter, DefaultForceScatter), DefaultForce.y + Random.Range(-DefaultForceScatter, DefaultForceScatter));

        NewHPP.GetComponent<Rigidbody>().AddForce(force);

    }

    public override void EntityDeadCheck()
    {
        if (curHP <= 0 && !isDead)
        {
            curHP = 0;
            ChangeToDeadState();

            curNormalRoom.enemyCnt--;

            isDead = true;

        }
    }

    public void DeadEnd()//Anim끝나고 실행
    {
        if (hpBar.gameObject.activeSelf)
        {
            hpBar.gameObject.SetActive(false);
        }

       
        EnemyPoolManager.Instance.ReturnEnemy(gameObject, stat.characterName);

        gameObject.SetActive(false);
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

        isDead = false;

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

        isDead = false;
        curNormalRoom = room;
        ChangeToIdleState();

    }
}
