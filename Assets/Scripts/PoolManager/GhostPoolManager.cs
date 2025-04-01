using System.Collections.Generic;
using UnityEngine;

public class GhostPoolManager : MonoBehaviour
{
    public static GhostPoolManager Instance { get; private set; }

    [SerializeField]
    private GameObject ghostPrefab;
    [SerializeField]
    private int initialPoolSize = 5;

    private Queue<GameObject> ghostPool = new Queue<GameObject>();

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
        InitializePool();
    }
    public void InitializePool()
    {
        ghostPool = new Queue<GameObject>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewGhost();
        }
    }

    private void CreateNewGhost()
    {
        GameObject ghostObject = Instantiate(ghostPrefab);
        ghostObject.SetActive(false);
        ghostPool.Enqueue(ghostObject);
    }

    public GameObject GetGhost()
    {
        if(ghostPool.Count == 0)
        {
            CreateNewGhost();
        }
        GameObject ghost = ghostPool.Dequeue();

        return ghost;
    }

    public void ReturnGhost(GameObject gameObject)
    {
        ghostPool.Enqueue(gameObject);
    }

}
