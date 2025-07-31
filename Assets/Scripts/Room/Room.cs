using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Room : MonoBehaviour
{
    [SerializeField]
    public GameManager gameManager;

    [SerializeField]
    protected Vector2Int roomPos;
    [SerializeField]
    protected List<Vector2Int> doors = new(); // 각 방의 문 정보


    [Header("Door"),Tooltip("0 : Top, 1: Right, 2:Bottom, 3:Left")]
    [SerializeField]
    protected List<Door> doorObj = new();

    [Tooltip("0 : Top, 1: Right, 2:Bottom, 3:Left")]
    public Vector3[] localSpawnpoints;
    Vector3[] globalSpawnpoints;

    Vector3 curSpawnpoints;

    [Header("RoomSize")]
    [SerializeField]
    Transform BottomLeft;
    [SerializeField]
    Transform TopRight;

    [HideInInspector]
    public float totalWidth;
    [HideInInspector]
    public float totalHeight;
    [HideInInspector]
    public Vector2 centerPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Awake()
    {
        

        //Debug.Log(transform.position);

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

        OpenDoor();

        MapSizeCalculate();

    }

    virtual public void SetMoveSpawn(Vector2Int currentPos, Vector2Int destination)
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


        totalWidth = Mathf.Abs(BottomLeft.localPosition.x)+Mathf.Abs(TopRight.localPosition.x);
        totalHeight = Mathf.Abs(BottomLeft.localPosition.y)+Mathf.Abs(TopRight.localPosition.y);

        //Debug.Log($"현재 맵 : {roomPos}");
        //Debug.Log($"전체 타일맵 크기: 가로 {totalWidth}, 세로 {totalHeight}");
        //Debug.Log($"좌표 범위: 좌측 하단 ({BottomLeft.position}), 우측 상단 ({TopRight.position})");

        centerPos = new Vector2((BottomLeft.position.x + TopRight.position.x) /2f, (BottomLeft.position.y + TopRight.position.y)/2f);
    }

    public void OpenDoor()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        foreach (var door in doors)
        {
            if (door == directions[0])
            {
                doorObj[0].DoorActive(true);
                doorObj[0].InitializeDoor(roomPos, roomPos + door);
            }
            else if (door == directions[1])
            {
                doorObj[1].DoorActive(true);
                doorObj[1].InitializeDoor(roomPos, roomPos + door);

            }
            else if (door == directions[2])
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

        RoomManager roomManager = FindFirstObjectByType<RoomManager>();
        roomManager.GenerateMiniMapforTilemap(GetComponentsInChildren<Tilemap>());
    }

    public void ClosedDoor()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        foreach (var door in doors)
        {
            if (door == directions[0])
            {
                doorObj[0].DoorActive(false);
                doorObj[0].InitializeDoor(roomPos, roomPos + door);
            }
            else if (door == directions[1])
            {
                doorObj[1].DoorActive(false);
                doorObj[1].InitializeDoor(roomPos, roomPos + door);

            }
            else if (door == directions[2])
            {
                doorObj[2].DoorActive(false);
                doorObj[2].InitializeDoor(roomPos, roomPos + door);

            }
            else if (door == directions[3])
            {
                doorObj[3].DoorActive(false);
                doorObj[3].InitializeDoor(roomPos, roomPos + door);
            }
        }
        RoomManager roomManager = FindFirstObjectByType<RoomManager>();
        roomManager.GenerateMiniMapforTilemap(GetComponentsInChildren<Tilemap>());
    }
    
    public virtual void OnDrawGizmos()
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
