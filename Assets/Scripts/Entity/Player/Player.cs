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

    private void Start()
    {
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
            hpText.text = $"{curHP}/{maxHP}";
            StartCoroutine("InvincibleCoolTime");
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
            gameObject.SetActive(false);
        }
    }
}
