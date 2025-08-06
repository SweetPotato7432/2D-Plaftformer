using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolRequestEnemy
{
    public EnemyType wantType;
    public GameObject prefap;
    public int amount;
}

[System.Serializable]
public struct PoolRequestEffect
{
    public EffectType wantType;
    public GameObject prefap;
    public int amount;
}

[System.Serializable]
public struct PoolRequestItem
{
    public ItemType wantType;
    public GameObject prefap;
    public int amount;
}

public delegate GameObject DelegateInstantiate(string targetName/*, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent, Space coord*/);
public delegate void DelegateReturnPool(GameObject target);

public class PoolManager : MonoBehaviour
{
    [SerializeField]
    private PoolRequestEnemy[] requestEnemies = new PoolRequestEnemy[0];
    [SerializeField]
    private PoolRequestEffect[] requestEffects = new PoolRequestEffect[0];
    [SerializeField]
    private PoolRequestItem[] requestItems = new PoolRequestItem[0];

    public static event DelegateInstantiate OnInstantiate;
    public static event DelegateReturnPool OnReturnPool;

    Dictionary<string, Queue<GameObject>> poolDictionary = new();

    Transform rootTransform;

    public bool IsInit { get; set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnInstantiate -= InstantiateFromPool;
        OnInstantiate += InstantiateFromPool;

        OnReturnPool -= ReleaseToPool;        
        OnReturnPool += ReleaseToPool;

        rootTransform = new GameObject("PoolRoot").transform;

        foreach (var currentEnemy in requestEnemies)
        {
            RegistrationFromObject(currentEnemy.prefap, currentEnemy.amount);
        }
        foreach(var currentEffect in requestEffects)
        {
            RegistrationFromObject(currentEffect.prefap, currentEffect.amount);
        }
        foreach(var currentItem in requestItems)
        {
            RegistrationFromObject(currentItem.prefap, currentItem.amount);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        OnInstantiate -= InstantiateFromPool;
        OnReturnPool -= ReleaseToPool;
    }

    // 풀 추가
    void RegistrationFromObject(GameObject prefab, int amount)
    {
        //GameObject result;
        //GameObject[] result = new GameObject[] { };
        if (prefab is null) { return; }

        string originalName = prefab.name;//.Replace("(Clone)","");
        Queue<GameObject> targetQueue;
        Transform currentRoot;
        if (!poolDictionary.TryGetValue(originalName, out targetQueue))
        {
            poolDictionary.Add(originalName, targetQueue = new());
            // 새로만든 타입의 이름을 가진 오브젝트를 만들어서 루트오브젝트에 넣기
            currentRoot = new GameObject(originalName).transform;
            currentRoot.SetParent(rootTransform);
        }
        else
        {
            currentRoot = GetRoot(originalName);
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject result = Instantiate(prefab);
            result.name = originalName;
            Registration(result, targetQueue, currentRoot);

        }
    }

    void Registration(GameObject target, Queue<GameObject> queue, Transform root)
    {
        if (target is null) return;

        target.SetActive(false);
        queue.Enqueue(target);
        target.transform.SetParent(root);

        if (target.TryGetComponent(out IPoolable poolComponent))
        {
            // 삭제 될때 다시 돌아갈 곳 저장
            poolComponent.RootQueue = queue;
        }
    }

    Transform GetRoot(string key)
    {
        return rootTransform.Find(key);
    }

    //풀에서 꺼내서 사용                                                                                               //space : 좌표계의 기준을 정한다.
    //GameObject InstantiateFromPool(string key, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent, Space coord)
    GameObject InstantiateFromPool(string key)
    {
        GameObject instance = GetPoolInstance(key);

        // 인스턴스가 없음
        if (instance is null) return null;

        return instance;

    }

    private GameObject GetPoolInstance(string key)
    {
        if (poolDictionary.TryGetValue(key, out Queue<GameObject> queue))
        {
            if (queue.TryDequeue(out GameObject result))
            {
                if (queue.Count == 0) // 마지막 Pool
                {
                    RegistrationFromObject(result, 5); // 부족하면 5개 추가
                }

                return result;
            }

        }

        return null;
    }

    void ReleaseToPool(GameObject target)
    {
        if (target?.TryGetComponent(out IPoolable asPool) ?? false)
        {
            Queue<GameObject> rootQueue = asPool.RootQueue;
            if (rootQueue is not null)
            {
                asPool.ReturnPool();
                rootQueue.Enqueue(target);
                target.SetActive(false);
                target.transform.parent = GetRoot(target.name);
                asPool.ReturnPool();
                return;
            }
        }
        //큐에 넣지 못하거나 IPoolable이 아니면 파괴
        Destroy(target);
    }

    public static GameObject ClaimInstantiate(string key)
    {
        return OnInstantiate?.Invoke(key);
    }
    public static void ClaimReturnPool(GameObject target)
    {
        OnReturnPool?.Invoke(target);
    }

}
