using Microlight.MicroBar;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : Entity
{
    [SerializeField]
    MicroBar hpBar;
    [SerializeField]
    TMP_Text hpText;

    public PlayerInfo stat;
    PlayerController controller;

    bool isInvinsible = false;
    bool isDead = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {


    }

    private void Start()
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

        controller = GetComponent<PlayerController>();

        if (hpBar != null) hpBar.Initialize(maxHP);
        hpText.text = $"{curHP}/{maxHP}";
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    public override void TakeDamage(float damage)
    {
        if (!isInvinsible)
        {
            base.TakeDamage(damage);

            if (hpBar != null) hpBar.UpdateBar(curHP,false,UpdateAnim.Damage);
            if (curHP <= 0)
            {
                curHP = 0;
            }
                hpText.text = $"{curHP}/{maxHP}";
            StartCoroutine("InvincibleCoolTime");
        }
    }

    public override void TakeHeal(float heal)
    {
        base.TakeHeal(heal);
        if (hpBar != null)
        {
            
            hpBar.SetNewMaxHP(maxHP,true);
            hpBar.UpdateBar(curHP, false, UpdateAnim.Heal);
        }
        if(curHP >= maxHP)
        {
            curHP = maxHP;
        }
        hpText.text = $"{curHP}/{maxHP}";
    }

    public override void ChangeStatus(Status wantType, float value)
    {
        switch (wantType)
        {
            case Status.HP:
                maxHP += value;
                TakeHeal(value);
                if (curHP > maxHP)
                    curHP = maxHP;
                break;
            case Status.ATK:
                atk += value;
                break;
            case Status.SPEED:
                moveSpeed += value;
                controller.moveSpeed += value;
                break;
        }
    }


    IEnumerator InvincibleCoolTime()
    {
        controller.EnableInvinsible();
        isInvinsible = true;
        yield return new WaitForSeconds(1f);

        controller.DisableInvinsible();
        isInvinsible = false;
    }
    public override void EntityDeadCheck()
    {
        if (curHP <= 0)
        {
            curHP = 0;
            state = States.DEAD;

            controller.DeadStart();

            Invoke("GameOver", 0.5f);

            isDead = true;
        }
    }

    void GameOver()
    {
        FindAnyObjectByType<UIManager>().GameOverUIActive();

    }


}
