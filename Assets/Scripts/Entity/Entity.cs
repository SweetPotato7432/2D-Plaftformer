using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public enum States
    {
        NONE,
        IDLE,
        MOVE,
        ATTACK,
        DEAD
    }

    public States state;

    public int id;
    protected string characterName;
    protected float maxHP;
    public float curHP;
    protected bool attackType;// 0 : 근접, 1 : 원거리
    protected float attackRange;
    protected float atk;
    protected float attackSpeed;
    protected float moveSpeed;
    protected float maxJumpHeight;
    protected float minJumpHeight;
    protected float timeToJumpApex;

    virtual public void Update()
    {
        EntityDeadCheck();
    }

    virtual public void TakeDamage(float damage)
    {
        Debug.Log("takeDamage"+damage);
        curHP -= damage;
    }

    virtual public void TakeHeal(float heal)
    {
        Debug.Log("takeHeal" + heal);
        curHP += heal;
    }

    public void Initialize(int id, string characterName, float maxHP, bool attackType, float attackRange, float atk, float atkSpeed, float moveSpeed, float maxJumpHeight, float minJumpHeight, float timeToJumpApex)
    {
        //this.id = id;
        this.characterName = characterName;
        this.maxHP = maxHP;
        this.curHP = maxHP;
        this.attackType = attackType;
        this.attackRange = attackRange;
        this.atk = atk;
        this.attackSpeed = atkSpeed;
        this.moveSpeed = moveSpeed;
        this.maxJumpHeight = maxJumpHeight;
        this.minJumpHeight = minJumpHeight;
        this.timeToJumpApex = timeToJumpApex;
    }

    public void Initialize(float maxHP)
    {
        curHP = maxHP;
    }

    public abstract void EntityDeadCheck();
}
