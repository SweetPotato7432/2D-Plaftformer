using UnityEngine;

public class Item : MonoBehaviour
{


    public int id;
    public string itemName;
    public int rarity;
    public DropItemInfo.EffectType effectType;
    public int effectStatus;
    public string effect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int id, string itemName, int rarity, DropItemInfo.EffectType effectType, int effectStatus, string effect)
    {
        this.id = id;
        this.itemName = itemName;
        this.rarity = rarity;
        this.effectType = effectType;
        this.effectStatus = effectStatus;
        this.effect = effect;
    }

    //È¹µæ ÆË¾÷ Ã¢
}
