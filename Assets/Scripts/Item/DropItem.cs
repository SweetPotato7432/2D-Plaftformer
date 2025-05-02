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
        // �÷��̾�� �ε�������, �������� �� ������ ������, Ȱ��ȭ Ű�� ������ ��ȣ�ۿ�.
        // �÷��̾�� �ε������� �� �˾�â ����
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
