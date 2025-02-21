using UnityEngine;

public class Entity : MonoBehaviour
{
    float id;
    string characterName;
    float maxHP;
    float curHP;
    int attackType;// 0 : 근접, 1 : 원거리
    float attackRange;
    float atk;
    float atkSpeed;
    float moveSpeed;
    float jumpForce;

    virtual public void Update()
    {
        if (curHP <= 0)
        {
            curHP = 0;
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("takeDamage"+damage);
        curHP -= damage;
    }

    public void Initialize(float id, string characterName, float maxHP, int attackType, float attackRange, float atk, float atkSpeed, float moveSpeed, float jumpForce)
    {
        this.id = id;
        this.characterName = characterName;
        this.maxHP = maxHP;
        this.curHP = maxHP;
        this.attackType = attackType;
        this.attackRange = attackRange;
        this.atk = atk;
        this.atkSpeed = atkSpeed;
        this.moveSpeed = moveSpeed;
        this.jumpForce = jumpForce;
    }
}
