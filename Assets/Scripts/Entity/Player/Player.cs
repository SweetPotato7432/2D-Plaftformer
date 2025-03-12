using UnityEngine;

public class Player : Entity
{
    public PlayerInfo stat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        stat = GameManager.Instance.PlayerStatInitialize(id);

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
    void Update()
    {
        
    }
}
