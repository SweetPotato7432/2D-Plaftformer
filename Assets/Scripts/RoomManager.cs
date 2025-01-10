using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private int roomAmount = 10;    // 생성할 방의 개수
    [SerializeField] private GameObject roomPrefab;  // 방 프리팹
    [SerializeField] private int mapWidth = 20;      // 맵의 가로 크기
    [SerializeField] private int mapHeight = 20;     // 맵의 세로 크기

    private GameObject[,] roomArray; // 방들을 저장할 배열
    private List<Vector2Int> createdRooms = new();  // 생성된 방들의 리스트

    void Start()
    {
        roomArray = new GameObject[mapWidth, mapHeight];
        GenerateMap();
    }

    void GenerateMap()
    {
        // 방을 생성할 초기 위치(중앙) 설정
        Vector2Int startPosition = new Vector2Int(mapWidth / 2, mapHeight / 2);
        createdRooms.Add(startPosition);
        roomArray[startPosition.x, startPosition.y] = CreateRoom(startPosition);

        // 방 생성 과정
        while (createdRooms.Count < roomAmount)
        {
            Vector2Int newRoomPos = GetRandomConnectedRoomPosition();
            if (newRoomPos != Vector2Int.zero)
            {
                createdRooms.Add(newRoomPos);
                roomArray[newRoomPos.x, newRoomPos.y] = CreateRoom(newRoomPos);

                // 방과 방을 연결할 때, 문을 추가
                ConnectRooms(newRoomPos);
            }
        }
    }

    // 방을 생성하는 함수
    GameObject CreateRoom(Vector2Int position)
    {
        GameObject room = Instantiate(roomPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        room.name = $"Room ({position.x}, {position.y})";
        return room;
    }

    // 생성된 방들과 연결된 새로운 방을 랜덤으로 선택하는 함수
    Vector2Int GetRandomConnectedRoomPosition()
    {
        // 이미 생성된 방 중 하나를 랜덤으로 선택
        Vector2Int selectedRoom = createdRooms[UnityEngine.Random.Range(0, createdRooms.Count)];

        // 상하좌우로 연결할 수 있는 후보 좌표
        List<Vector2Int> candidates = new List<Vector2Int>();

        // 상하좌우로 방을 생성할 수 있는지 체크
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var direction in directions)
        {
            Vector2Int newRoomPos = selectedRoom + direction;

            // 문을 생성할 확률을 부여
            if (IsValidRoomPosition(newRoomPos) && roomArray[newRoomPos.x, newRoomPos.y] == null)
            {
                // 문을 열 확률 25%
                if (Random.Range(0f, 1f) < 0.25f)
                {
                    candidates.Add(newRoomPos);
                }
            }
        }

        // 후보가 없으면 문이 없는 곳이라도 방을 만들어야 하므로 그때는 모든 방향에서 방을 만들 수 있게 한다.
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

        // 후보가 있으면 랜덤으로 하나 선택하여 반환
        if (candidates.Count > 0)
        {
            return candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }

        return Vector2Int.zero; // 더 이상 연결할 수 없으면 zero 반환
    }

    // 주어진 좌표가 맵 내에 존재하는지 확인하는 함수
    bool IsValidRoomPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < mapWidth && position.y >= 0 && position.y < mapHeight;
    }

    // 새로 생성된 방과 기존 방을 연결하는 함수
    void ConnectRooms(Vector2Int newRoomPos)
    {
        // 방 간 문이 연결되는 방향을 지정
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        List<Vector2Int> connectedDirections = new();

        foreach (var direction in directions)
        {
            Vector2Int adjacentPos = newRoomPos + direction;

            if (IsValidRoomPosition(adjacentPos) && roomArray[adjacentPos.x, adjacentPos.y] != null)
            {
                // 문을 연결할 방향을 추가
                connectedDirections.Add(direction);

                // 문 위치를 디버그로 표시
                Vector3 roomCenter = new Vector3(newRoomPos.x, newRoomPos.y, 0);
                Vector3 doorPosition = roomCenter + new Vector3(direction.x, direction.y, 0);

                // 해당 방향으로 선을 그려서 문 위치를 표시
                Debug.DrawLine(roomCenter, doorPosition, Color.red, 5f); // 5초 동안 빨간 선으로 문 표시
            }
        }

        // 연결된 방향 중 하나를 선택하여 문을 추가하도록 할 수 있음
        if (connectedDirections.Count > 0)
        {
            // 문을 연결할 방향을 확률적으로 선택
            Vector2Int directionWithDoor = connectedDirections[UnityEngine.Random.Range(0, connectedDirections.Count)];
            // 예시: roomArray[newRoomPos.x, newRoomPos.y].GetComponent<Room>().AddDoor(directionWithDoor);
            Debug.Log($"Door opened at {newRoomPos} in direction {directionWithDoor}");
        }
    }
}
