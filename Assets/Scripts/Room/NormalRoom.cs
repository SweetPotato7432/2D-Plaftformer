using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalRoom : Room
{
    [System.Serializable]
    public struct EnemySpawnInfo
    {
        public string tag;
        public Vector3 localSpawnpoint;
        [HideInInspector]
        public Vector3 globalSpawnPoint;
    }

    [System.Serializable]
    public struct RewardSpawnInfo
    {
        public Vector3 localSpawnpoint;
        [HideInInspector]
        public Vector3 globalSpawnPoint;
    }

    public int enemyCnt;

    public bool isRoomClear = false;
    bool isOnBattle = false;
    
    [Header("SpawnEnemy")]
    public EnemySpawnInfo[] spawnInfos;
    //public Vector3[] enemyLocalSpawnpoints;
    //[SerializeField]
    //Vector3[] enemyGlobalSpawnpoints;

    [Header("SpawnReward")]
    [SerializeField]
    RewardSpawnInfo rewardSpawn;

    public override void Awake()
    {
        base.Awake();

        for(int i = 0; i < spawnInfos.Length; i++ )
        {
            spawnInfos[i].globalSpawnPoint = spawnInfos[i].localSpawnpoint + transform.position;
        }

        rewardSpawn.globalSpawnPoint = rewardSpawn.localSpawnpoint + transform.position;
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
            if (enemyCnt <= 0)
            {
                Debug.Log("클리어");

                //방 클리어시 드랍 아이템 확률 드랍.
                if (Random.value <= 0.2f)
                {
                    DropItem dropItem = DropItemPoolManager.Instance.GetDropItem();

                    dropItem.transform.position = rewardSpawn.globalSpawnPoint;

                    // 아이템 개수에 맞게 수정
                    int len = GameManager.Instance.DropItemLength();

                    int rarity = dropItem.CalculateRarityFromDropItem();
                    
                    List<int> candidateIds = GameManager.Instance.dropItemRarityGroups[rarity];
                    int selectedItemId = candidateIds[Random.Range(0, candidateIds.Count)];

                    Debug.Log($"r : {rarity}, id : {selectedItemId}");

                    dropItem.InitalizeDropItem(selectedItemId);
                }

                
                
                isRoomClear = true;
                isOnBattle = false;
                OpenDoor();
            }
        }
        else
        {
        }
    }

    public void SpawnEnemy()
    {
        for (int i = 0; i < spawnInfos.Length; i++)
        {
            GameObject enemyObj = EnemyPoolManager.Instance.GetEnemy(spawnInfos[i].tag);
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemyObj != null)
            {
                enemyObj.transform.position = spawnInfos[i].globalSpawnPoint;
                enemy.InitializeEnemy(this);
                enemyObj.SetActive(true);
                enemyCnt++;
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

        
        {
            Gizmos.color = Color.yellow;
            float size = .3f;

            Vector3 globalSpawnpointPos = (Application.isPlaying) ? rewardSpawn.globalSpawnPoint : rewardSpawn.localSpawnpoint + transform.position;
            Gizmos.DrawLine(globalSpawnpointPos - Vector3.up * size, globalSpawnpointPos + Vector3.up * size);
            Gizmos.DrawLine(globalSpawnpointPos - Vector3.left * size, globalSpawnpointPos + Vector3.left * size);
        }
    }
}
