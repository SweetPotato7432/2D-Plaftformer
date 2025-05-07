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

    private Queue<GameObject> dropItemPool = new Queue<GameObject>();

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
        dropItemPool = new Queue<GameObject>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewDropItem();
        }
    }

    private void CreateNewDropItem()
    {
        GameObject dropItemObject = Instantiate(dropItemPrefab);
        dropItemObject.SetActive(false);
        dropItemPool.Enqueue(dropItemObject);
    }

    public GameObject GetDropItem()
    {
        if (dropItemPool.Count == 0)
        {
            CreateNewDropItem();
        }
        GameObject dropItem = dropItemPool.Dequeue();

        return dropItem;
    }

    public void ReturnDropItem(GameObject gameObject)
    {
        dropItemPool.Enqueue(gameObject);
        gameObject.SetActive(false );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
