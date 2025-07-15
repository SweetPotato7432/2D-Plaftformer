using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{

    public static EnemyPoolManager Instance { get; private set; }

    [System.Serializable]
    public struct EnemyPool
    {
        public string id;
        public GameObject prefab;
        public int size;
    }

    public List<EnemyPool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    // EnemyPool 검색용
    private Dictionary<string, EnemyPool> enemyPoolLookup;

    private void Awake()
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

        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        enemyPoolLookup = new Dictionary<string, EnemyPool>();


        foreach (EnemyPool pool in pools)
        {
            InitializePool(pool);
            enemyPoolLookup[pool.id] = pool;
        }
    }

    public void InitializePool(EnemyPool pool)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();
        for (int i = 0; i < pool.size; i++)
        {
            CreateNewEnemy(pool,  objectPool);
        }
        poolDictionary.Add(pool.id, objectPool);
    }

    private void CreateNewEnemy(EnemyPool pool, Queue<GameObject> objectPool)
    {
        GameObject enemyObject = Instantiate(pool.prefab);
        //enemyObject.GetComponent<Enemy>().InitializeEnemy(); // Assuming Enemy has an InitializeEnemy method
        enemyObject.SetActive(false);
        objectPool.Enqueue(enemyObject);
    }

    public GameObject GetEnemy(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }
        Queue<GameObject> objectPool = poolDictionary[tag];

        if (objectPool.Count == 0)
        {
            if(!enemyPoolLookup.TryGetValue(tag, out EnemyPool pool))
            {
                Debug.LogError($"[EnemyPoolManager] {tag}에 해당하는 EnemyPool이 존재하지 않습니다.");
                return null;
            }
            CreateNewEnemy(pool, objectPool);
        }
        GameObject enemy = objectPool.Dequeue();
        return enemy;
    }

    public void ReturnEnemy(GameObject gameObject,string tag)
    {
        poolDictionary[tag].Enqueue(gameObject);
    }

}
