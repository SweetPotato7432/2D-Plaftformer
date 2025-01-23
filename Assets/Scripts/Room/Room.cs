using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    protected Vector2Int roomPos;
    [SerializeField]
    protected List<Vector2Int> doors = new(); // 각 방의 문 정보
    [Header("Door"),Tooltip("0 : Top, 1: Right, 2:Bottom, 3:Left")]
    [SerializeField]
    protected List<Door> doorObj = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
            }
            else if (door == directions[1])
            {
                doorObj[1].DoorActive(true);
            }
            else if(door == directions[2])
            {
                doorObj[2].DoorActive(true);
            }
            else if (door == directions[3])
            {
                doorObj[3].DoorActive(true);
            }
        }
    }
}
