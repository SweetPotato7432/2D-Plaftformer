using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class Room : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    protected Vector2Int roomPos;
    [SerializeField]
    protected List<Vector2Int> doors = new(); // 각 방의 문 정보
    [Header("Door"),Tooltip("0 : Top, 1: Right, 2:Bottom, 3:Left")]
    [SerializeField]
    protected List<Door> doorObj = new();

    [Tooltip("0 : Top, 1: Right, 2:Bottom, 3:Left")]
    public Vector3[] localSpawnpoints;
    [SerializeField]
    Vector3[] globalSpawnpoints;

    [SerializeField]
    Vector3 curSpawnpoints;

    float totalWidth;
    float totalHeight;
    Vector2 centerPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        

        Debug.Log(transform.position);

        globalSpawnpoints = new Vector3[localSpawnpoints.Length];
        for (int i = 0; i < localSpawnpoints.Length; i++)
        {
            globalSpawnpoints[i] = localSpawnpoints[i] + transform.position;
        }

        gameManager = Resources.FindObjectsOfTypeAll<GameManager>().FirstOrDefault();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IntializeRoomData(Vector2Int pos, List<Vector2Int> doors)
    {
        this.roomPos = pos;
        this.doors = doors.ToList();

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        foreach (var door in doors)
        {
            if (door == directions[0])
            {
                doorObj[0].DoorActive(true);
                doorObj[0].InitializeDoor(roomPos,roomPos + door);
            }
            else if (door == directions[1])
            {
                doorObj[1].DoorActive(true);
                doorObj[1].InitializeDoor(roomPos,roomPos + door);

            }
            else if(door == directions[2])
            {
                doorObj[2].DoorActive(true);
                doorObj[2].InitializeDoor(roomPos, roomPos + door);

            }
            else if (door == directions[3])
            {
                doorObj[3].DoorActive(true);
                doorObj[3].InitializeDoor(roomPos, roomPos + door);
            }
        }
        MapSizeCalculate();

    }

    public void SetMoveSpawn(Vector2Int currentPos, Vector2Int destination)
    {
        Vector2Int roomDir = destination - currentPos;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right,  Vector2Int.down, Vector2Int.left };

        if (roomDir == directions[0])
        {
            curSpawnpoints = globalSpawnpoints[2];
        }
        else if(roomDir == directions[1])
        {
            curSpawnpoints = globalSpawnpoints[3];
        }
        else if(roomDir == directions[2])
        {
            curSpawnpoints = globalSpawnpoints[0];
        }
        else if(roomDir == directions[3])
        {
            curSpawnpoints = globalSpawnpoints[1];
        }

        gameManager.PlayerMoveRoom(curSpawnpoints,totalWidth,totalHeight,centerPos);
        
    }

    void MapSizeCalculate()
    {
        float tileSize = 16f;
        Tilemap[] tilemaps = this.GetComponentsInChildren<Tilemap>();

        if (tilemaps.Length == 0)
        {
            Debug.LogError("부모 오브젝트에 Tilemap이 없습니다!");
            return;
        }

        Vector3 minPos = new Vector3(float.MaxValue, float.MaxValue, 0);
        Vector3 maxPos = new Vector3(float.MinValue, float.MinValue, 0);

        // 모든 Tilemap을 순회하며 좌측 하단, 우측 상단 좌표 찾기
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap == null) continue;

            BoundsInt bounds = tilemap.cellBounds;
            Vector3 tilemapWorldPos = tilemap.transform.position;

            Vector3 worldMin = tilemapWorldPos + new Vector3(bounds.min.x, bounds.min.y, 0);
            Vector3 worldMax = tilemapWorldPos + new Vector3(bounds.max.x, bounds.max.y, 0);

            minPos.x = Mathf.Min(minPos.x, worldMin.x);
            minPos.y = Mathf.Min(minPos.y, worldMin.y);
            maxPos.x = Mathf.Max(maxPos.x, worldMax.x);
            maxPos.y = Mathf.Max(maxPos.y, worldMax.y);
        }

        // 전체 맵의 가로, 세로 크기 계산
        totalWidth = (maxPos.x - minPos.x) * tileSize;
        totalHeight = (maxPos.y - minPos.y) * tileSize;

        Debug.Log($"현재 맵 : {roomPos}");
        Debug.Log($"전체 타일맵 크기: 가로 {totalWidth}, 세로 {totalHeight}");
        Debug.Log($"좌표 범위: 좌측 하단 ({minPos.x}, {minPos.y}), 우측 상단 ({maxPos.x}, {maxPos.y})");

        centerPos = new Vector2((maxPos.x + minPos.x)/2f, (maxPos.y - minPos.y)/2f);
    }
    
    private void OnDrawGizmos()
    {
        if (localSpawnpoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localSpawnpoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalSpawnpoints[i] : localSpawnpoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
