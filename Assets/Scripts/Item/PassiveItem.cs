using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class PassiveItem : Item
{
    public PassiveItemInfo stat;

    public GameObject popup;

    public TMP_Text itemNameText;
    public TMP_Text itemEffectText;

    public PassiveItemInfo.EffectType effectType;

    Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override public void Start()
    {
        base.Start();
        //stat = GameManager.Instance.DropItemInfoInitialize(id);
        InitalizePassiveItem(stat.id);
    }

    public void InitalizePassiveItem(int id)
    {
        stat = GameManager.Instance.PassiveItemInfoInitialize(id);

        //Initialize(stat.id, stat.itemName, stat.rarity, stat.effectType, stat.effectStatus, stat.effect);
        Initialize(stat.id, stat.itemName, stat.rarity, stat.effectStatus, stat.effect);

        effectType = stat.effectType;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);

        string spriteKey = $"Assets/Addressable/PassiveItem/PassiveItem_{stat.id}.asset";

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

    public void ActiveEffect(Player player)
    {
        switch (effectType)
        {
            case PassiveItemInfo.EffectType.Health:
                
                PassiveItemPoolManager.Instance.ReturnPassiveItem(this);
                break;
            case PassiveItemInfo.EffectType.Attack:
                PassiveItemPoolManager.Instance.ReturnPassiveItem(this);

                break;
            case PassiveItemInfo.EffectType.Speed:
                PassiveItemPoolManager.Instance.ReturnPassiveItem(this);

                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾�� �ε�������, �������� �� ������ ������, Ȱ��ȭ Ű�� ������ ��ȣ�ۿ�.
        // �÷��̾�� �ε������� �� �˾�â ����
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            popup.SetActive(true);

            // ������ ȿ�� �����ε� Passive�� ���� �ٸ��� �����ϳ�?
            PlayerInput.OnActivePickupItemEffect -= ActiveEffect;
            PlayerInput.OnActivePickupItemEffect += ActiveEffect;
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
