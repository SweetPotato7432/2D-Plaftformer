using System.Collections.Generic;
using UnityEngine;

public class PassiveItemPoolManager : MonoBehaviour
{
    public static PassiveItemPoolManager Instance { get; private set; }

    [SerializeField]
    private GameObject passiveItemPrefab;

    [SerializeField]
    private int initialPoolSize = 3;

    private Queue<PassiveItem> passiveItemPool = new Queue<PassiveItem>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //DontDestroyOnLoad(gameObject);
        InitializePool();
    }

    public void InitializePool()
    {
        passiveItemPool = new Queue<PassiveItem>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewPassiveItem();
        }
    }
    private void CreateNewPassiveItem()
    {
        GameObject passiveItemObject = Instantiate(passiveItemPrefab);
        passiveItemObject.SetActive(false);
        passiveItemPool.Enqueue(passiveItemObject.GetComponent<PassiveItem>());
    }

    public PassiveItem GetPassiveItem()
    {
        if (passiveItemPool.Count == 0)
        {
            CreateNewPassiveItem();
        }
        PassiveItem passiveItem = passiveItemPool.Dequeue();

        return passiveItem;
    }

    public void ReturnPassiveItem(PassiveItem gameObject)
    {
        passiveItemPool.Enqueue(gameObject);
        gameObject.gameObject.SetActive(false);
    }
}
