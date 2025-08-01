using System.Collections.Generic;
using UnityEngine;

public class EffectPoolManager : MonoBehaviour
{
    public static EffectPoolManager Instance { get; private set; }

    [System.Serializable]
    public struct EffectPool
    {
        public string id;
        public GameObject prefab;
        public int size;
    }

    public List<EffectPool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    //EffectPool 검색용
    private Dictionary<string, EffectPool> effectPoolLookup;

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
        //DontDestroyOnLoad(gameObject);

        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        effectPoolLookup = new Dictionary<string, EffectPool>();

        foreach (EffectPool pool in pools)
        {
            InitializePool(pool);
            effectPoolLookup[pool.id] = pool;
        }
    }

    public void InitializePool(EffectPool pool)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();
        for (int i = 0; i < pool.size; i++)
        {
            CreateNewEffect(pool, objectPool);
        }
        poolDictionary.Add(pool.id, objectPool);
    }

    private void CreateNewEffect(EffectPool pool, Queue<GameObject> objectPool)
    {
        GameObject effectObject = Instantiate(pool.prefab);
        effectObject.SetActive(false);
        objectPool.Enqueue(effectObject);
    }

    public GameObject GetEffect(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }
        Queue<GameObject> objectPool = poolDictionary[tag];

        if (objectPool.Count == 0)
        {
            if (!effectPoolLookup.TryGetValue(tag, out EffectPool pool))
            {
                Debug.LogError($"[EnemyPoolManager] {tag}에 해당하는 EnemyPool이 존재하지 않습니다.");
                return null;
            }
            CreateNewEffect(pool, objectPool);
        }
        GameObject effect = objectPool.Dequeue();
        return effect;
    }

    public void ReturnEffect(GameObject gameObject, string tag)
    {
        poolDictionary[tag].Enqueue(gameObject);
    }

}
