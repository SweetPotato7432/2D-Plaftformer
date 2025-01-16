using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private int roomAmount;    // 생성할 방의 개수
    [SerializeField] private GameObject roomPrefab;  // 방 프리팹
    [SerializeField] private int mapWidth;      // 맵의 가로 크기
    [SerializeField] private int mapHeight;     // 맵의 세로 크기

    private GameObject[,] roomArray;                 // 방들을 저장할 배열
    // roomDoors의 키 : 방의 좌표값, 밸류 : 옆 방 방향 설정
    // 연결된 방 좌표 구하는 법 : 키(방의 좌표 값) + 밸류(옆 방 방향)
    private Dictionary<Vector2Int, List<Vector2Int>> roomDoors = new(); // 각 방의 문 정보
    private List<Vector2Int> createdRooms = new();   // 생성된 방들의 리스트
    private HashSet<Vector2Int> blockedPositions = new(); // 금지된 방 위치
    private List<Vector2Int> endRooms = new(); // 끝방 위치

    void Start()
    {
        roomArray = new GameObject[mapWidth, mapHeight];
        while (createdRooms.Count < roomAmount)
        {
            ResetMap();
            GenerateMap();
            Debug.Log($"Successfully generated {createdRooms.Count} room!");
            //return;

            // 실패 시 맵 초기화 및 재시작
            //Debug.LogWarning($"Failed to generate enough rooms. Retrying ...");
        }

        // 생성된 문 파악 후 연결된 방이 없는 문 파기
        
        ValidateDoors();

        endRooms = FindEndRooms();
        Debug.Log("endRooms Cnt" + endRooms.Count);
        foreach (var room in endRooms)
        {
            Debug.Log(room);
        }
        if(endRooms.Count < 3)
        {
            List<Vector2Int> tempCreatedRooms = createdRooms.ToList();

            foreach (var room in tempCreatedRooms)
            {
                if (!endRooms.Contains(room))
                {
                    if (AddEndRoom(room))
                    {
                        endRooms = FindEndRooms();
                        Debug.Log("endRooms Cnt" + endRooms.Count);
                        foreach (var room1 in endRooms)
                        {
                            Debug.Log(room1);
                        }
                        if (endRooms.Count >= 3)
                        {
                            break;
                        }
                    }
                }
            }
        }

        PlaceSpeacialRoom();
    }

    void GenerateMap()
    {
        Vector2Int startPosition = new Vector2Int(mapWidth / 2, mapHeight / 2);
        CreateRoom(startPosition);

        int maxAttempts = roomAmount * 10; // 안전 장치: 시도 횟수 제한
        int attempts = 0;

        while (createdRooms.Count < roomAmount && attempts < maxAttempts)
        {
            Vector2Int newRoomPos = GetRandomConnectedRoomPosition();
            attempts++;

            if (newRoomPos != Vector2Int.zero)
            {
                CreateRoom(newRoomPos);
            }
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogError($"Failed to generate the required number of rooms ({roomAmount}). Generated {createdRooms.Count} rooms.");
        }
    }

    void CreateRoom(Vector2Int position)
    {
        GameObject room = Instantiate(roomPrefab, new Vector3(position.x-mapWidth/2, position.y-mapHeight/2, 0), Quaternion.identity);
        room.name = $"Room ({position.x}, {position.y})";
        roomArray[position.x, position.y] = room;

        List<Vector2Int> doors = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var direction in directions)
        {
            Vector2Int adjacentPos = position + direction;
            if (createdRooms.Contains(adjacentPos))
            {
                doors.Add(direction);
                // 양방향 문 연결 보장
                if (IsValidRoomPosition(adjacentPos) && roomArray[adjacentPos.x, adjacentPos.y] != null)
                {
                    if (!roomDoors[adjacentPos].Contains(-direction))
                    {
                        roomDoors[adjacentPos].Add(-direction);
                    }
                }
            }
            else if (!blockedPositions.Contains(adjacentPos) && Random.Range(0f, 1f) < 0.5f) // 방이 막혔는지 확인, 50%의 확률로 생성
            {
                doors.Add(direction);

                // 양방향 문 연결 보장
                if (IsValidRoomPosition(adjacentPos) && roomArray[adjacentPos.x, adjacentPos.y] != null)
                {
                    if (!roomDoors[adjacentPos].Contains(-direction))
                    {
                        roomDoors[adjacentPos].Add(-direction);
                    }
                }
            }
            else
            {
                blockedPositions.Add(adjacentPos);
            }
        }

        roomDoors[position] = doors;
        createdRooms.Add(position);
    }

    // 퍼뜨릴수 있는 문 기준으로 방을 생성
    Vector2Int GetRandomConnectedRoomPosition()
    {
        Vector2Int selectedRoom = createdRooms[Random.Range(0, createdRooms.Count)];
        List<Vector2Int> doors = roomDoors[selectedRoom];

        List<Vector2Int> candidates = new List<Vector2Int>();
        foreach (var direction in doors)
        {
            Vector2Int newRoomPos = selectedRoom + direction;

            if (IsValidRoomPosition(newRoomPos) && roomArray[newRoomPos.x, newRoomPos.y] == null && !blockedPositions.Contains(newRoomPos))
            {
                candidates.Add(newRoomPos);
            }
        }

        if (candidates.Count > 0)
        {
            return candidates[Random.Range(0, candidates.Count)];
        }

        Debug.LogWarning($"No valid position found for new room connected to {selectedRoom}");
        return Vector2Int.zero;
    }

    // 방이 크기를 벗어났는지 확인
    bool IsValidRoomPosition(Vector2Int position)
    {
        if (position.x < 0 || position.x >= mapWidth || position.y < 0 || position.y >= mapHeight)
        {
            Debug.LogWarning($"Position {position} is out of bounds.");
            return false;
        }

        return true;
    }

    void ValidateDoors()
    {
        foreach (var room in createdRooms)
        {
            List<Vector2Int> doorsToRemove = new List<Vector2Int>();
            foreach (var doorDirection in roomDoors[room])
            {
                Vector2Int adjacentPosition = room + doorDirection;

                // 연결된 방이 없거나 유효하지 않은 경우
                if (!IsValidRoomPosition(adjacentPosition) || roomArray[adjacentPosition.x, adjacentPosition.y] == null)
                {
                    doorsToRemove.Add(doorDirection);
                }
            }

            // 잘못된 문 제거
            foreach (var door in doorsToRemove)
            {
                Debug.Log($"Destroy{room}/{door}");
                roomDoors[room].Remove(door);
            }
        }
    }

    // 끝방 찾기
    List<Vector2Int> FindEndRooms()
    {
        List<Vector2Int> endRooms = new();
        foreach(var room in createdRooms)
        {
            if(roomDoors[room].Count == 1)
            {
                endRooms.Add(room);
            }
        }

        Vector2Int originRoom = new Vector2Int(mapWidth/2, mapHeight/2);

        if (endRooms.Contains(originRoom))
        {
            endRooms.Remove(originRoom);
        }

        return endRooms;
    }

    bool AddEndRoom(Vector2Int baseRoom)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var direction in directions)
        {
            Vector2Int adjacentPosition = baseRoom + direction;
            // 막힌 방에 강제로 방생성, 근처에 방이 하나여야함.
            if (blockedPositions.Contains(adjacentPosition)&& ClosedRoomCnt(adjacentPosition)==1)
            {
                CreateRoom(adjacentPosition);
                Debug.Log($"강제 생성 끝방 : {adjacentPosition}");
                ValidateDoors();
                foreach (var door in roomDoors[adjacentPosition])
                {
                    Debug.Log(door);
                }

                return true;

            }
        }

        return false;
    }

    void PlaceSpeacialRoom()
    {
        // 가장 먼 방 보스방, 나머지 방들 랜덤으로 보물방, 상점

        List<Vector2Int> tempEndRooms = endRooms.ToList();

        Dictionary<Vector2Int, int> endRoomDistance = new Dictionary<Vector2Int, int>();
        foreach (var room in tempEndRooms)
        {
            int distance = Mathf.Abs(room.x - mapWidth / 2) + Mathf.Abs(room.y - mapHeight / 2);

            endRoomDistance.Add(room, distance);
        }

        var furthestRoom = endRoomDistance.Aggregate((maxRoom, nextRoom) => nextRoom.Value > maxRoom.Value ? nextRoom : maxRoom).Key;
        Debug.Log(furthestRoom);
        tempEndRooms.Remove(furthestRoom);

        int random = Random.Range(0, tempEndRooms.Count);
        Debug.Log($"보물방 : {tempEndRooms[random]}");
        tempEndRooms.Remove(tempEndRooms[random]);

        random = Random.Range(0, tempEndRooms.Count);
        Debug.Log($"상점 : {tempEndRooms[random]}");
        tempEndRooms.Remove(tempEndRooms[random]);
    }

    int ClosedRoomCnt(Vector2Int baseRoom)
    {
        int cnt = 0;
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var direction in directions)
        {
            Vector2Int adjacentPosition = baseRoom + direction;
            if (createdRooms.Contains(adjacentPosition))
            {
                cnt++;
            }
        }

        return cnt;
    }

    private void ResetMap()
    {
        foreach (var roomPos in createdRooms)
        {
            Destroy(roomArray[roomPos.x, roomPos.y]);
        }

        // 데이터 초기화
        createdRooms.Clear();
        roomDoors.Clear();
        blockedPositions.Clear();
        roomArray = new GameObject[mapWidth, mapHeight];

    }
}