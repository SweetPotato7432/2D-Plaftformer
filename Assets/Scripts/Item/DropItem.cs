using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class DropItem : Item
{
    public DropItemInfo stat;

    public GameObject popup;

    public TMP_Text itemNameText;
    public TMP_Text itemEffectText;

    public DropItemInfo.EffectType effectType;

    Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();


    private void Awake()
    {
        

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override public void Start()
    {
        base.Start();
        //stat = GameManager.Instance.DropItemInfoInitialize(id);
        InitalizeDropItem(id);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitalizeDropItem(int id)
    {
        stat = GameManager.Instance.DropItemInfoInitialize(id);

        //Initialize(stat.id, stat.itemName, stat.rarity, stat.effectType, stat.effectStatus, stat.effect);
        Initialize(stat.id, stat.itemName, stat.rarity, stat.effectStatus, stat.effect);

        effectType = stat.effectType;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);

        string spriteKey = $"Assets/Addressable/DropItem/DropItem_{stat.id}.asset";

        if (spriteCache.TryGetValue(spriteKey, out var cachedSprite))
        {
            renderer.sprite = cachedSprite;
            gameObject.SetActive(true);
        }
        else
        {
            Addressables.LoadAssetAsync<Sprite>(spriteKey).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    spriteCache[spriteKey] = handle.Result;
                    renderer.sprite = handle.Result;
                }
                else
                {
                    Debug.LogWarning($"Sprite 로딩 실패: {spriteKey}");
                    // renderer.sprite = defaultSprite;
                }
                gameObject.SetActive(true);
            };
        }

        popup.SetActive(false);

        itemNameText.text = stat.itemName;
        itemEffectText.text = stat.effect;
    }

    public void ActiveEffect(Player player)
    {
        switch (effectType)
            {
                case DropItemInfo.EffectType.Heal:
                    player.TakeHeal(stat.effectStatus);
                    DropItemPoolManager.Instance.ReturnDropItem(this);
                    break;
                case DropItemInfo.EffectType.Gold:
                    DropItemPoolManager.Instance.ReturnDropItem(this);

                    break;
            }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 부딪혔을때, 아이템의 상세 설명이 나오고, 활성화 키를 누르면 상호작용.
        // 플레이어와 부딪힐때는 상세 팝업창 출현
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            popup.SetActive(true);

            PlayerInput.OnActivePickupItemEffect -= ActiveEffect;
            PlayerInput.OnActivePickupItemEffect += ActiveEffect;


            //switch (effectType)
            //{
            //    case DropItemInfo.EffectType.Heal:
            //        player.TakeHeal(stat.effectStatus);
            //        DropItemPoolManager.Instance.ReturnDropItem(this);
            //        break;
            //    case DropItemInfo.EffectType.Gold:
            //        break;
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            PlayerInput.OnActivePickupItemEffect -= ActiveEffect;
            popup.SetActive(false);
        }
    }

}
