using System.Collections.Generic;
using UnityEngine;

public class NormalRoom : Room
{
    HashSet<Enemy> enemyList = new HashSet<Enemy>();

    public bool isRoomClear;
    
    [Header("SpawnEnemy")]    
    public Vector3[] enemyLocalSpawnpoints;
    [SerializeField]
    Vector3[] enemyGlobalSpawnpoints;

    public override void Awake()
    {
        base.Awake();

        enemyGlobalSpawnpoints = new Vector3[enemyLocalSpawnpoints.Length];
        for (int i = 0; i < enemyLocalSpawnpoints.Length; i++)
        {
            enemyGlobalSpawnpoints[i] = enemyLocalSpawnpoints[i] + transform.position;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (enemyLocalSpawnpoints != null)
        {
            Gizmos.color = Color.green;
            float size = .3f;

            for (int i = 0; i < enemyLocalSpawnpoints.Length; i++)
            {
                Vector3 globalSpawnpointPos = (Application.isPlaying) ? enemyGlobalSpawnpoints[i] : enemyLocalSpawnpoints[i] + transform.position;
                Gizmos.DrawLine(globalSpawnpointPos - Vector3.up * size, globalSpawnpointPos + Vector3.up * size);
                Gizmos.DrawLine(globalSpawnpointPos - Vector3.left * size, globalSpawnpointPos + Vector3.left * size);
            }
        }
    }
}
