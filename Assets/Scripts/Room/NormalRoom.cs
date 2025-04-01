using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalRoom : Room
{
    [System.Serializable]
    public struct SpawnInfo
    {
        public string tag;
        public Vector3 localSpawnpoint;
        [HideInInspector]
        public Vector3 globalSpawnPoint;
    }

    HashSet<GameObject> enemyList = new HashSet<GameObject>();

    public bool isRoomClear = false;
    bool isOnBattle = false;
    
    [Header("SpawnEnemy")]
    public SpawnInfo[] spawnInfos;
    //public Vector3[] enemyLocalSpawnpoints;
    //[SerializeField]
    //Vector3[] enemyGlobalSpawnpoints;

    public override void Awake()
    {
        base.Awake();

        for(int i = 0; i < spawnInfos.Length; i++ )
        {
            spawnInfos[i].globalSpawnPoint = spawnInfos[i].localSpawnpoint + transform.position;
        }

        //enemyGlobalSpawnpoints = new Vector3[enemyLocalSpawnpoints.Length];
        //for (int i = 0; i < enemyLocalSpawnpoints.Length; i++)
        //{
        //    enemyGlobalSpawnpoints[i] = enemyLocalSpawnpoints[i] + transform.position;
        //}
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnBattle)
        {
            if (enemyList.Count <= 0)
            {

            }
        }
    }

    public void SpawnEnemy()
    {
        for (int i = 0; i < spawnInfos.Length; i++)
        {
            GameObject Enemy = EnemyPoolManager.Instance.GetEnemy(spawnInfos[i].tag);
            if (Enemy != null)
            {
                Enemy.transform.position = spawnInfos[i].globalSpawnPoint;
                Enemy.SetActive(true);
                enemyList.Add(Enemy);
            }

        }
    }

    public override void SetMoveSpawn(Vector2Int currentPos, Vector2Int destination)
    {
        base.SetMoveSpawn(currentPos, destination);

        if (!isRoomClear)
        {
            isOnBattle = true;
            ClosedDoor();
            SpawnEnemy();
        }
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (spawnInfos!=null)
        {
            Gizmos.color = Color.green;
            float size = .3f;

            for (int i = 0; i < spawnInfos.Length; i++)
            {
                Vector3 globalSpawnpointPos = (Application.isPlaying) ? spawnInfos[i].globalSpawnPoint : spawnInfos[i].localSpawnpoint + transform.position;
                Gizmos.DrawLine(globalSpawnpointPos - Vector3.up * size, globalSpawnpointPos + Vector3.up * size);
                Gizmos.DrawLine(globalSpawnpointPos - Vector3.left * size, globalSpawnpointPos + Vector3.left * size);
            }
        }

        //if (enemyLocalSpawnpoints != null)
        //{
        //    Gizmos.color = Color.green;
        //    float size = .3f;

        //    for (int i = 0; i < enemyLocalSpawnpoints.Length; i++)
        //    {
        //        Vector3 globalSpawnpointPos = (Application.isPlaying) ? enemyGlobalSpawnpoints[i] : enemyLocalSpawnpoints[i] + transform.position;
        //        Gizmos.DrawLine(globalSpawnpointPos - Vector3.up * size, globalSpawnpointPos + Vector3.up * size);
        //        Gizmos.DrawLine(globalSpawnpointPos - Vector3.left * size, globalSpawnpointPos + Vector3.left * size);
        //    }
        //}
    }
}
