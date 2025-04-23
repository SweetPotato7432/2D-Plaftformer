using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Unity.VisualScripting.Member;
using static UnityEditor.Experimental.GraphView.GraphView;

public class RoomManager : MonoBehaviour
{
    public enum RoomType
    {
        NONE,
        START,
        NORMAL,
        TREASURE,
        SHOP,
        BOSS
    }

    [SerializeField] private int roomAmount;    // 생성할 방의 개수
    [SerializeField] private int mapWidth;      // 맵의 가로 크기
    [SerializeField] private int mapHeight;     // 맵의 세로 크기

    //private GameObject[,] roomArray;                 // 방Prefap들을 저장할 배열(디버그용)
    // roomDoors의 키 : 방의 좌표값, 밸류 : 옆 방 방향 설정
    // 연결된 방 좌표 구하는 법 : 키(방의 좌표 값) + 밸류(옆 방 방향)
    private Dictionary<Vector2Int, List<Vector2Int>> roomDoors = new(); // 각 방의 문 정보
    private Dictionary<Vector2Int, RoomType> roomTypes = new();
    private Dictionary<Vector2Int, int> roomDistances = new(); // 각 방들이 시작 방으로부터 얼마나 떨어져 있는지
    private List<Vector2Int> createdRooms = new();   // 생성된 방들의 리스트
    private HashSet<Vector2Int> blockedPositions = new(); // 금지된 방 위치
    private List<Vector2Int> endRooms = new(); // 끝방 위치

    [SerializeField]
    private Dictionary<Vector2Int, Room> roomDic = new();

    [SerializeField]
    private GameObject[] normalRoomPrefaps;
    [SerializeField]
    private GameObject[] specialRoomPrefaps;

    [Header("Minimap")]
    [SerializeField]
    private Tilemap miniMapTilemap;
    [SerializeField]
    private TileBase whiteTile;

    [Header("Worldmap")]
    [SerializeField]
    private UIManager uiManager;


    void Start()
    {
        //roomArray = new GameObject[mapWidth, mapHeight];

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

        foreach (var room in roomDistances)
        {
            Debug.Log($"키 : {room.Key}, 밸류 : {room.Value}");
        }
        // 방 실제 생성
        GeneratePlayableRoom();

        // 방 미니맵 생성
        Tilemap[] sourceTilemaps = FindSourceTilemapsWithLayer("Ground");
        GenerateMiniMap(sourceTilemaps);

        uiManager.GenerateWorldmap(roomTypes);
    }

    void GenerateMap()
    {
        Vector2Int startPosition = new Vector2Int(mapWidth / 2, mapHeight / 2);
        CreateRoom(startPosition);
        roomTypes[startPosition] = RoomType.START;

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
        // Instantiate 부분은 추후에 삭제(디버깅용)
        //GameObject room = Instantiate(roomPrefab, new Vector3(position.x - mapWidth / 2, position.y - mapHeight / 2, 0), Quaternion.identity);
        //room.name = $"Room ({position.x}, {position.y})";
        //roomArray[position.x, position.y] = room;

        int distance = 0;

        roomTypes.Add(position, RoomType.NORMAL);
        createdRooms.Add(position);


        List<Vector2Int> doors = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        
        // 각 
        foreach (var direction in directions)
        {
            Vector2Int adjacentPos = position + direction;
            if (createdRooms.Contains(adjacentPos)) // 방향에 방이 존재한다면
            {
                
                doors.Add(direction);
                // 양방향 문 연결 보장
                if (IsValidRoomPosition(adjacentPos))
                {
                    if (!roomDoors[adjacentPos].Contains(-direction))
                    {
                        roomDoors[adjacentPos].Add(-direction);
                    }
                }
                // 시작방 에서 부터의 거리 저장
                if (roomDistances[adjacentPos] >= distance)
                {
                    distance = roomDistances[adjacentPos]+1;
                }
            }
            else if (!blockedPositions.Contains(adjacentPos) && Random.Range(0f, 1f) < 0.5f) // 방이 막혔는지 확인, 50%의 확률로 생성
            {
                doors.Add(direction);

                // 양방향 문 연결은 새롭게 생긴 방에서만 시도한다. 상대 방에도 연결을 해주니 문제 없음, 내 방에만 문 생성
            }
            else
            {
                blockedPositions.Add(adjacentPos);
            }
        }

        roomDoors[position] = doors;
        roomDistances[position] = distance;
        
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

            if (IsValidRoomPosition(newRoomPos) && !createdRooms.Contains(newRoomPos)/*roomArray[newRoomPos.x, newRoomPos.y] == null*/ && !blockedPositions.Contains(newRoomPos))
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
                if (!IsValidRoomPosition(adjacentPosition) || !createdRooms.Contains(adjacentPosition)/*roomArray[adjacentPosition.x, adjacentPosition.y] == null*/)
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
            int distance = roomDistances[room];

            endRoomDistance.Add(room, distance);
        }

        var furthestRoom = endRoomDistance.Aggregate((maxRoom, nextRoom) => nextRoom.Value > maxRoom.Value ? nextRoom : maxRoom).Key;
        roomTypes[furthestRoom] = RoomType.BOSS;
        Debug.Log($"보스 : {furthestRoom}");
        tempEndRooms.Remove(furthestRoom);

        int random = Random.Range(0, tempEndRooms.Count);
        roomTypes[tempEndRooms[random]] = RoomType.TREASURE;
        Debug.Log($"보물방 : {tempEndRooms[random]}");
        tempEndRooms.Remove(tempEndRooms[random]);

        random = Random.Range(0, tempEndRooms.Count);
        roomTypes[tempEndRooms[random]] = RoomType.SHOP;
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
        //foreach (var roomPos in createdRooms)
        //{
        //    Destroy(roomArray[roomPos.x, roomPos.y]);
        //}
        //roomArray = new GameObject[mapWidth, mapHeight];

        // 데이터 초기화
        createdRooms.Clear();
        roomDoors.Clear();
        blockedPositions.Clear();
        roomTypes.Clear();


    }

    private void GeneratePlayableRoom()
    {
        GameObject prefab = null;

        List<GameObject> tempNormalRooms = new List<GameObject>();

        //실제 플레이 가능 한 방을 생성
        foreach (var room in roomTypes)
        {
            if (tempNormalRooms.Count <= 0) 
            {
                tempNormalRooms = normalRoomPrefaps.ToList();
            }

            switch (room.Value)
            {
                case RoomType.START:
                    prefab = specialRoomPrefaps[0];
                    break;
                case RoomType.NORMAL:
                    int rand = Random.Range(0,tempNormalRooms.Count);

                    prefab = tempNormalRooms[rand];
                    tempNormalRooms.RemoveAt(rand);
                    //prefab = normalRoomPrefaps[1];
                    break;
                case RoomType.TREASURE:
                    prefab = specialRoomPrefaps[1];
                    break;
                case RoomType.SHOP:
                    prefab = specialRoomPrefaps[2];
                    break;
                case RoomType.BOSS:
                    prefab = specialRoomPrefaps[3];
                    break;
            }
            Vector3 roomPos = new Vector3((room.Key.x - mapWidth / 2) * 180, (room.Key.y - mapHeight / 2) * 180, 0);
            GameObject tempRoom = Instantiate(prefab,roomPos,Quaternion.identity);
            Vector2Int pos = room.Key;
            roomDic.Add(pos, tempRoom.GetComponent<Room>());
            roomDic[pos].IntializeRoomData(pos, roomDoors[pos]);
            
            tempRoom.name = $"Room ({room.Key.x}, {room.Key.y})";
            //tempRoom.transform.position = new Vector3((room.Key.x - mapWidth / 2) * 180, (room.Key.y - mapHeight/2) * 180, 0);
        }
    }

    public void setMoveRoomDestination(Vector2Int currentPos, Vector2Int destination)
    {
        roomDic[destination].SetMoveSpawn(currentPos, destination);
        uiManager.RevealedWorldmap(destination);
    }


    // Ground 태그가 붙은 모든 Tilemap 자동 검색
    Tilemap[] FindSourceTilemapsWithLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogWarning($"레이어 \"{layerName}\" 를 찾을 수 없습니다.");
            return new Tilemap[0];
        }

        List<Tilemap> result = new List<Tilemap>();
        Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);

        foreach (var tilemap in allTilemaps)
        {
            if (tilemap.gameObject.layer == layer)
            {
                tilemap.CompressBounds();
                result.Add(tilemap);
            }
        }

        return result.ToArray();
    }

    void GenerateMiniMap(Tilemap[] sourceTilemaps)
    {
        miniMapTilemap.ClearAllTiles();

        foreach (var source in sourceTilemaps)
        {
            BoundsInt bounds = source.cellBounds;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int localPos = new Vector3Int(x, y, 0);
                    if (!source.HasTile(localPos)) continue;

                    Vector3 worldPos = source.CellToWorld(localPos); // 타일의 실제 월드 위치
                    Vector3Int miniMapCellPos = miniMapTilemap.WorldToCell(worldPos); // 미니맵 타일맵 좌표로 변환

                    // 중복 체크 후 배치
                    if (!miniMapTilemap.HasTile(miniMapCellPos))
                    {
                        miniMapTilemap.SetTile(miniMapCellPos, whiteTile);
                    }
                }
            }
        }

        Debug.Log("미니맵 타일들이 실제 위치에 맞춰 배치되었습니다!");
    }

    public void GenerateMiniMapforTilemap(Tilemap[] sourceTilemaps)
    {
        int layer = LayerMask.NameToLayer("Ground");
        if (layer == -1)
        {
            Debug.LogWarning($"레이어 \"{"Ground"}\" 를 찾을 수 없습니다.");
            return;
        }

        List<Tilemap> result = new List<Tilemap>();

        foreach (var tilemap in sourceTilemaps)
        {
            if (tilemap.gameObject.layer == layer)
            {
                tilemap.CompressBounds();
                result.Add(tilemap);
            }
        }

        foreach(var source in result)
        {
            BoundsInt bounds = source.cellBounds;

            ClearMiniMapBounds(bounds, source);
        }
        foreach (var source in result)
        {
            BoundsInt bounds = source.cellBounds;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int localPos = new Vector3Int(x, y, 0);
                    if (!source.HasTile(localPos)) continue;

                    Vector3 worldPos = source.CellToWorld(localPos); // 타일의 실제 월드 위치
                    Vector3Int miniMapCellPos = miniMapTilemap.WorldToCell(worldPos); // 미니맵 타일맵 좌표로 변환

                    // 중복 체크 후 배치
                    if (!miniMapTilemap.HasTile(miniMapCellPos))
                    {
                        miniMapTilemap.SetTile(miniMapCellPos, whiteTile);
                    }
                }
            }
        }
        Debug.Log("미니맵 타일들이 갱신되었습니다.");
    }

    void ClearMiniMapBounds(BoundsInt bounds, Tilemap referenceTilemap)
    {
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                Vector3 worldPos = referenceTilemap.CellToWorld(pos);
                Vector3Int miniMapCellPos = miniMapTilemap.WorldToCell(worldPos);

                miniMapTilemap.SetTile(miniMapCellPos, null); // 타일 제거
            }
        }
    }
}