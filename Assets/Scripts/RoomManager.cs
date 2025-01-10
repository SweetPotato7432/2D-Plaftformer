using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private int roomAmount = 10;    // ������ ���� ����
    [SerializeField] private GameObject roomPrefab;  // �� ������
    [SerializeField] private int mapWidth = 20;      // ���� ���� ũ��
    [SerializeField] private int mapHeight = 20;     // ���� ���� ũ��

    private GameObject[,] roomArray; // ����� ������ �迭
    private List<Vector2Int> createdRooms = new();  // ������ ����� ����Ʈ

    void Start()
    {
        roomArray = new GameObject[mapWidth, mapHeight];
        GenerateMap();
    }

    void GenerateMap()
    {
        // ���� ������ �ʱ� ��ġ(�߾�) ����
        Vector2Int startPosition = new Vector2Int(mapWidth / 2, mapHeight / 2);
        createdRooms.Add(startPosition);
        roomArray[startPosition.x, startPosition.y] = CreateRoom(startPosition);

        // �� ���� ����
        while (createdRooms.Count < roomAmount)
        {
            Vector2Int newRoomPos = GetRandomConnectedRoomPosition();
            if (newRoomPos != Vector2Int.zero)
            {
                createdRooms.Add(newRoomPos);
                roomArray[newRoomPos.x, newRoomPos.y] = CreateRoom(newRoomPos);

                // ��� ���� ������ ��, ���� �߰�
                ConnectRooms(newRoomPos);
            }
        }
    }

    // ���� �����ϴ� �Լ�
    GameObject CreateRoom(Vector2Int position)
    {
        GameObject room = Instantiate(roomPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        room.name = $"Room ({position.x}, {position.y})";
        return room;
    }

    // ������ ���� ����� ���ο� ���� �������� �����ϴ� �Լ�
    Vector2Int GetRandomConnectedRoomPosition()
    {
        // �̹� ������ �� �� �ϳ��� �������� ����
        Vector2Int selectedRoom = createdRooms[UnityEngine.Random.Range(0, createdRooms.Count)];

        // �����¿�� ������ �� �ִ� �ĺ� ��ǥ
        List<Vector2Int> candidates = new List<Vector2Int>();

        // �����¿�� ���� ������ �� �ִ��� üũ
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var direction in directions)
        {
            Vector2Int newRoomPos = selectedRoom + direction;

            // ���� ������ Ȯ���� �ο�
            if (IsValidRoomPosition(newRoomPos) && roomArray[newRoomPos.x, newRoomPos.y] == null)
            {
                // ���� �� Ȯ�� 25%
                if (Random.Range(0f, 1f) < 0.25f)
                {
                    candidates.Add(newRoomPos);
                }
            }
        }

        // �ĺ��� ������ ���� ���� ���̶� ���� ������ �ϹǷ� �׶��� ��� ���⿡�� ���� ���� �� �ְ� �Ѵ�.
        if (candidates.Count == 0 && createdRooms.Count < roomAmount)
        {
            foreach (var direction in directions)
            {
                Vector2Int newRoomPos = selectedRoom + direction;
                if (IsValidRoomPosition(newRoomPos) && roomArray[newRoomPos.x, newRoomPos.y] == null)
                {
                    candidates.Add(newRoomPos);
                }
            }
        }

        // �ĺ��� ������ �������� �ϳ� �����Ͽ� ��ȯ
        if (candidates.Count > 0)
        {
            return candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }

        return Vector2Int.zero; // �� �̻� ������ �� ������ zero ��ȯ
    }

    // �־��� ��ǥ�� �� ���� �����ϴ��� Ȯ���ϴ� �Լ�
    bool IsValidRoomPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < mapWidth && position.y >= 0 && position.y < mapHeight;
    }

    // ���� ������ ��� ���� ���� �����ϴ� �Լ�
    void ConnectRooms(Vector2Int newRoomPos)
    {
        // �� �� ���� ����Ǵ� ������ ����
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        List<Vector2Int> connectedDirections = new();

        foreach (var direction in directions)
        {
            Vector2Int adjacentPos = newRoomPos + direction;

            if (IsValidRoomPosition(adjacentPos) && roomArray[adjacentPos.x, adjacentPos.y] != null)
            {
                // ���� ������ ������ �߰�
                connectedDirections.Add(direction);

                // �� ��ġ�� ����׷� ǥ��
                Vector3 roomCenter = new Vector3(newRoomPos.x, newRoomPos.y, 0);
                Vector3 doorPosition = roomCenter + new Vector3(direction.x, direction.y, 0);

                // �ش� �������� ���� �׷��� �� ��ġ�� ǥ��
                Debug.DrawLine(roomCenter, doorPosition, Color.red, 5f); // 5�� ���� ���� ������ �� ǥ��
            }
        }

        // ����� ���� �� �ϳ��� �����Ͽ� ���� �߰��ϵ��� �� �� ����
        if (connectedDirections.Count > 0)
        {
            // ���� ������ ������ Ȯ�������� ����
            Vector2Int directionWithDoor = connectedDirections[UnityEngine.Random.Range(0, connectedDirections.Count)];
            // ����: roomArray[newRoomPos.x, newRoomPos.y].GetComponent<Room>().AddDoor(directionWithDoor);
            Debug.Log($"Door opened at {newRoomPos} in direction {directionWithDoor}");
        }
    }
}
