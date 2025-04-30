using UnityEngine;

public class DropItem : Item
{
    public DropItemInfo stat;

    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

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

}
