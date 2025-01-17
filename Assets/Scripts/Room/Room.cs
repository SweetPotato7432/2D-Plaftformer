using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    protected Vector2Int roomPos;
    [SerializeField]
    protected List<Vector2Int> doors = new(); // 각 방의 문 정보

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
    }
}
