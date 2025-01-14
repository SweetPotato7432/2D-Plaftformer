using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private int roomAmount = 10;    // ������ ���� ����
    [SerializeField] private GameObject roomPrefab;  // �� ������
    [SerializeField] private int mapWidth = 20;      // ���� ���� ũ��
    [SerializeField] private int mapHeight = 20;     // ���� ���� ũ��

    private GameObject[,] roomArray;                 // ����� ������ �迭
    // roomDoors�� Ű : ���� ��ǥ��, ��� : �� �� ���� ����
    // ����� �� ��ǥ ���ϴ� �� : Ű(���� ��ǥ ��) + ���(�� �� ����)
    private Dictionary<Vector2Int, List<Vector2Int>> roomDoors = new(); // �� ���� �� ����
    private List<Vector2Int> createdRooms = new();   // ������ ����� ����Ʈ
    private HashSet<Vector2Int> blockedPositions = new(); // ������ �� ��ġ

    void Start()
    {
        roomArray = new GameObject[mapWidth, mapHeight];
        while (createdRooms.Count < roomAmount)
        {
            ResetMap();
            GenerateMap();
            Debug.Log($"Successfully generated {createdRooms.Count} room!");
            //return;

            // ���� �� �� �ʱ�ȭ �� �����
            //Debug.LogWarning($"Failed to generate enough rooms. Retrying ...");
        }

        // ������ �� �ľ� �� ����� ���� ���� �� �ı�
        
        ValidateDoors();

    }



    void GenerateMap()
    {
        Vector2Int startPosition = new Vector2Int(mapWidth / 2, mapHeight / 2);
        CreateRoom(startPosition);

        int maxAttempts = roomAmount * 10; // ���� ��ġ: �õ� Ƚ�� ����
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
        GameObject room = Instantiate(roomPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        room.name = $"Room ({position.x}, {position.y})";
        roomArray[position.x, position.y] = room;

        List<Vector2Int> doors = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var direction in directions)
        {
            Vector2Int adjacentPos = position + direction;

            if (!blockedPositions.Contains(adjacentPos) && Random.Range(0f, 1f) < 0.5f)
            {
                doors.Add(direction);

                // ����� �� ���� ����
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

                // ����� ���� ���ų� ��ȿ���� ���� ���
                if (!IsValidRoomPosition(adjacentPosition) || roomArray[adjacentPosition.x, adjacentPosition.y] == null)
                {
                    doorsToRemove.Add(doorDirection);
                }
            }

            // �߸��� �� ����
            foreach (var door in doorsToRemove)
            {
                Debug.Log($"Destroy{room}/{door}");
                roomDoors[room].Remove(door);
            }
        }
    }

    private void ResetMap()
    {
        foreach (var roomPos in createdRooms)
        {
            Destroy(roomArray[roomPos.x, roomPos.y]);
        }

        // ������ �ʱ�ȭ
        createdRooms.Clear();
        roomDoors.Clear();
        blockedPositions.Clear();
        roomArray = new GameObject[mapWidth, mapHeight];

    }
}