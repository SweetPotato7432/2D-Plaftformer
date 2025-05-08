using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static EnemyPoolManager;

public class DropItemPoolManager : MonoBehaviour
{
    public static DropItemPoolManager Instance { get; private set; }

    [SerializeField]
    private GameObject dropItemPrefab;

    [SerializeField]
    private int initialPoolSize = 5;

    private Queue<DropItem> dropItemPool = new Queue<DropItem>();

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
        DontDestroyOnLoad(gameObject);
        InitializePool();
    }

    public void InitializePool()
    {
        dropItemPool = new Queue<DropItem>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewDropItem();
        }
    }

    private void CreateNewDropItem()
    {
        GameObject dropItemObject = Instantiate(dropItemPrefab);
        dropItemObject.SetActive(false);
        dropItemPool.Enqueue(dropItemObject.GetComponent<DropItem>());
    }

    public DropItem GetDropItem()
    {
        if (dropItemPool.Count == 0)
        {
            CreateNewDropItem();
        }
        DropItem dropItem = dropItemPool.Dequeue();

        return dropItem;
    }

    public void ReturnDropItem(DropItem gameObject)
    {
        dropItemPool.Enqueue(gameObject);
        gameObject.gameObject.SetActive(false );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
