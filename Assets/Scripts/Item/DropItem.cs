using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEditor.Progress;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class DropItem : Item
{
    public DropItemInfo stat;

    public GameObject popup;

    public TMP_Text itemNameText;
    public TMP_Text itemEffectText;

    Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();


    private void Awake()
    {
        

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override public void Start()
    {
        base.Start();
        //stat = GameManager.Instance.DropItemInfoInitialize(id);
        InitalizeDropItem(stat.id);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitalizeDropItem(int id)
    {
        stat = GameManager.Instance.DropItemInfoInitialize(id);

        Initialize(stat.id, stat.itemName, stat.rarity, stat.effectType, stat.effectStatus, stat.effect);

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
                    Debug.LogWarning($"Sprite �ε� ����: {spriteKey}");
                    // renderer.sprite = defaultSprite;
                }
                gameObject.SetActive(true);
            };
        }

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
                    DropItemPoolManager.Instance.ReturnDropItem(this);
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
