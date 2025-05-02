using TMPro;
using UnityEngine;

public class DropItem : Item
{
    public DropItemInfo stat;

    public GameObject popup;

    public TMP_Text itemNameText;
    public TMP_Text itemEffectText;

    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override public void Start()
    {
        base.Start();
        //stat = GameManager.Instance.DropItemInfoInitialize(id);
        InitalizeDropItem();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitalizeDropItem()
    {
        stat = GameManager.Instance.DropItemInfoInitialize(id);

        Initialize(stat.id, stat.itemName, stat.rarity, stat.effectType, stat.effectStatus, stat.effect);

        popup.SetActive(false);

        itemNameText.text = stat.itemName;
        itemEffectText.text = stat.effect;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 부딪혔을때, 아이템의 상세 설명이 나오고, 활성화 키를 누르면 상호작용.
        // 플레이어와 부딪힐때는 상세 팝업창 출현
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            popup.SetActive(true);


            switch (effectType)
            {
                case DropItemInfo.EffectType.Heal:
                    player.TakeHeal(stat.effectStatus);
                    break;
                case DropItemInfo.EffectType.Gold:
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            popup.SetActive(false);
        }
    }

}
