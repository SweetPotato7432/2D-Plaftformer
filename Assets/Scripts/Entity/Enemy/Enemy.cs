using UnityEngine;

public class Enemy : Entity 
{
    
    public MonsterInfo stat;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    virtual public void Start()
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
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }
}
