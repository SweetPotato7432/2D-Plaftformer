using UnityEngine;

public class Enemy : Entity 
{
    public MonsterInfo stat;
    public int id;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stat = GameManager.Instance.EnemeyStatInitialize(id);
        this.id = stat.id;
        this.characterName = stat.characterName;
        this.maxHP = stat.hp;
        this.curHP = stat.hp;
        this.attackType = stat.attackType;
        this.attackRange = stat.attackRange;
        this.attackSpeed = stat.attackSpeed;
        this.moveSpeed = stat.moveSpeed;
        this.jumpForce = stat.jumpForce;
        //Initialize(1, "test", 10, 1, 0, 1, 1, 1, 1);
    }

    // Update is called once per frame
    virtual public void Update()
    {
        base.Update();
    }
}
