using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour, ISceneInitializer
{


    [SerializeField] private int roomAmount;    // 생성할 방의 개수
    [SerializeField] private int mapWidth;      // 맵의 가로 크기
    [SerializeField] private int mapHeight;     // 맵의 세로 크기

    //private GameObject[,] roomArray;                 // 방Prefap들을 저장할 배열(디버그용)
    // roomDoors의 키 : 방의 좌표값, 밸류 : 옆 방 방향 설정
    // 연결된 방 좌표 구하는 법 : 키(방의 좌표 값) + 밸류(옆 방 방향)
    private Dictionary<Vector2Int, List<Vector2Int>> roomDoors = new(); // 각 방의 문 정보
    // 생성된 방의 좌표와 타입을 저장하는 Dictionary
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

    // 씬 초기화
    public IEnumerator InitializeScene()
    {
        InitializeRoomManager();
        yield break;
    }

    void Start()
    {
        //InitializeRoomManager();

        uiManager.GenerateWorldmap(roomTypes);

    }

    // RoomManager 초기화
    public void InitializeRoomManager()
    {
        //roomArray = new GameObject[mapWidth, mapHeight];

        while (createdRooms.Count < roomAmount)
        {
            // 방 초기화
            ResetMap();
            // 방 구조 생성
            GenerateMap();
            Debug.Log($"Successfully generated {createdRooms.Count} room!");
            //return;

            // 실패 시 맵 초기화 및 재시작
            //Debug.LogWarning($"Failed to generate enough rooms. Retrying ...");
        }

        // 생성된 문 파악 후 연결된 방이 없는 문 파기
        ValidateDoors();

        // 끝방 탐색
        endRooms = FindEndRooms();
        // 끝방의 개수가 지정 숫자보다 적다면
        if (endRooms.Count < 3)
        {
            // 추가적으로 방을 생성한다.
            List<Vector2Int> tempCreatedRooms = createdRooms.ToList();
            foreach (var room in tempCreatedRooms)
            {
                //생성된 방들 중에 끝방에 포함되지 않는 방을
                if (!endRooms.Contains(room))
                {
                    // 방 추가
                    if (AddEndRoom(room))
                    {
                        // 끝방 목록 갱신
                        endRooms = FindEndRooms();
                        // 끝방의 카운트가 지정 숫자에 도달하면 종료
                        if (endRooms.Count >= 3)
                        {
                            break;
                        }
                    }
                }
            }
        }

        // 생성된 끝방에 특수방 종료 추가
        PlaceSpeacialRoom();

        // 방 실제 생성
        GeneratePlayableRoom();

        // 방 미니맵 생성
        Tilemap[] sourceTilemaps = FindSourceTilemapsWithLayer("Ground");
        //GenerateMiniMap(sourceTilemaps);

    }

    // 맵구조 좌표 생성 시작
    void GenerateMap()
    {
        // 시작 지점을 총 맵 크기의 중심점에서 시작
        Vector2Int startPosition = new Vector2Int(mapWidth / 2, mapHeight / 2);
        // 방 생성 및 문 생성
        CreateRoom(startPosition);
        // 시작 방은 RoomType.Start로 설정
        roomTypes[startPosition] = RoomType.START;

        int maxAttempts = roomAmount * 10; // 안전 장치: 시도 횟수 제한
        int attempts = 0;

        // 방 개수가 원하는 개수를 충족할때까지 반복
        while (createdRooms.Count < roomAmount && attempts < maxAttempts)
        {
            // 이전에 생성한 문 위치를 기반으로 새로운 방 좌표를 구한다.
            Vector2Int newRoomPos = GetRandomConnectedRoomPosition();
            // 시도 횟수 증가
            attempts++;

            // 새 방 좌표가 0,0이 아니라면.
            if (newRoomPos != Vector2Int.zero)
            {
                // 방 및 문을 생성한다.
                CreateRoom(newRoomPos);
            }
        }

        //최대 회수를 넘어섰다면 오류 출력
        if (attempts >= maxAttempts)
        {
            Debug.LogWarning($"Failed to generate the required number of rooms ({roomAmount}). Generated {createdRooms.Count} rooms.");
        }
    }

    // 방 생성 및 문 생성
    void CreateRoom(Vector2Int position)
    {
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
                // 방향의 방이 최대 방 크기를 벗어나지 않으면
                if (IsValidRoomPosition(adjacentPos))
                {
                    // 방향의 방에 문이 존재하지 않는다면 
                    if (!roomDoors[adjacentPos].Contains(-direction))
                    {
                        // 추가
                        roomDoors[adjacentPos].Add(-direction);
                    }
                }
                // 시작방 에서 부터의 거리 저장
                if (roomDistances[adjacentPos] >= distance)
                {
                    distance = roomDistances[adjacentPos]+1;
                }
            }
            // 방이 막혔는지 확인, 50%의 확률로 생성
            else if (!blockedPositions.Contains(adjacentPos) && Random.Range(0f, 1f) < 0.5f) 
            {
                doors.Add(direction);
                // 양방향 문 연결은 새롭게 생긴 방에서만 시도한다.
                // 상대 방에도 연결을 해주니 문제 없음, 내 방에만 문 생성
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
        // 무작위로 선택된 방에
        Vector2Int selectedRoom = createdRooms[Random.Range(0, createdRooms.Count)];
        // 생성되어있는 문의 정보를 받아와
        List<Vector2Int> doors = roomDoors[selectedRoom];

        // 새 방 을 저장할 리스트
        List<Vector2Int> candidates = new List<Vector2Int>();
        // 문을 기반으로
        foreach (var direction in doors)
        {
            // 새방의 좌표를 구한다.
            Vector2Int newRoomPos = selectedRoom + direction;

            // 방이 맵의 크기를 벗어났는지 파악 && 이미 생성되지 않았는지 파악 && 막혀있지 않은지 파악
            if (IsValidRoomPosition(newRoomPos) && !createdRooms.Contains(newRoomPos) && !blockedPositions.Contains(newRoomPos))
            {
                // 새 방 좌표를 추가한다.
                candidates.Add(newRoomPos);
            }
        }
        // 만약 새롭게 추가될 방이 있다면
        if (candidates.Count > 0)
        {
            // 새방의 좌표들 중 하나를 반환한다.
            return candidates[Random.Range(0, candidates.Count)];
        }

        // 없다면 0,0 반환
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

    // 연결이 되지 않은 문 파기
    void ValidateDoors()
    {
        // 생성된 방
        foreach (var room in createdRooms)
        {
            // 제거될 문의 좌표를 저장할 리스트
            List<Vector2Int> doorsToRemove = new List<Vector2Int>();
            // 방의 문 좌표
            foreach (var doorDirection in roomDoors[room])
            {
                // 문이랑 연결되어야할 좌표를 구한다.
                Vector2Int adjacentPosition = room + doorDirection;

                // 방의 크기를 벗어난 좌표 || 방이 생성되어있지 않다면
                if (!IsValidRoomPosition(adjacentPosition) || !createdRooms.Contains(adjacentPosition))
                {
                    // 제거 목록에 추가
                    doorsToRemove.Add(doorDirection);
                }
            }

            // 잘못된 문 제거
            foreach (var door in doorsToRemove)
            {
                roomDoors[room].Remove(door);
            }
        }
    }

    // 끝방 찾기
    List<Vector2Int> FindEndRooms()
    {
        // 끝방의 정보를 저장할 리스트
        List<Vector2Int> endRooms = new();
        foreach(var room in createdRooms)
        {
            // 방 문이 1개인 경우
            if(roomDoors[room].Count == 1)
            {
                //끝방 정보 추가
                endRooms.Add(room);
            }
        }

        // 첫방의 좌표를 구해서 첫방이 끝방이라면 제외
        Vector2Int originRoom = new Vector2Int(mapWidth/2, mapHeight/2);
        if (endRooms.Contains(originRoom))
        {
            endRooms.Remove(originRoom);
        }

        return endRooms;
    }

    // 강제 끝방 생성
    bool AddEndRoom(Vector2Int baseRoom)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        // 각 방향을 탐색
        foreach (var direction in directions)
        {
            // 생성할 위치
            Vector2Int adjacentPosition = baseRoom + direction;
            // 막힌 방에 강제로 방생성, 근처에 방이 하나여야함.
            // 생성할 위치가 막혀있고 && 생성할 위치에 붙은 방이 1개라면
            if (blockedPositions.Contains(adjacentPosition)&& ClosedRoomCnt(adjacentPosition)==1)
            {
                // 방 생성
                CreateRoom(adjacentPosition);
                Debug.Log($"강제 생성 끝방 : {adjacentPosition}");
                // 연결 안된 방 파기
                ValidateDoors();

                return true;

            }
        }

        return false;
    }

    // 특수방 배치
    void PlaceSpeacialRoom()
    {
        // 가장 먼 방 보스방, 나머지 방들 랜덤으로 보물방, 상점

        // 임시 끝방 리스트
        List<Vector2Int> tempEndRooms = endRooms.ToList();

        // 시작 방부터 끝방의 거리 책정
        Dictionary<Vector2Int, int> endRoomDistance = new Dictionary<Vector2Int, int>();
        foreach (var room in tempEndRooms)
        {
            int distance = roomDistances[room];

            endRoomDistance.Add(room, distance);
        }

        // 가장 먼 방을 구한다.
        var furthestRoom = endRoomDistance.Aggregate((maxRoom, nextRoom) => nextRoom.Value > maxRoom.Value ? nextRoom : maxRoom).Key;
        // 가장 먼 방을 보스방으로 지정
        roomTypes[furthestRoom] = RoomType.BOSS;
        Debug.Log($"보스 : {furthestRoom}");
        // 지정한 방은 임시 끝방 목록에서 제거
        tempEndRooms.Remove(furthestRoom);

        // 남은 끝방들을 보물방으로 생성한다.
        int random = Random.Range(0, tempEndRooms.Count);
        roomTypes[tempEndRooms[random]] = RoomType.TREASURE;
        Debug.Log($"보물방 : {tempEndRooms[random]}");
        tempEndRooms.Remove(tempEndRooms[random]);

        // 상점으로 만들거 일단 보물방으로 대체해서 생성
        random = Random.Range(0, tempEndRooms.Count);
        roomTypes[tempEndRooms[random]] = RoomType.TREASURE;
        Debug.Log($"보물방 : {tempEndRooms[random]}");
        tempEndRooms.Remove(tempEndRooms[random]);
    }

    // baseRoom에 붙어있는 방의 개수 카운트
    int ClosedRoomCnt(Vector2Int baseRoom)
    {
        int cnt = 0;
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        // 각 방향에
        foreach (var direction in directions)
        {
            Vector2Int adjacentPosition = baseRoom + direction;
            // 방이 생성되어있다면
            if (createdRooms.Contains(adjacentPosition))
            {
                // 카운트
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

    // 실제 플레이할 방 생성
    private void GeneratePlayableRoom()
    {
        GameObject prefab = null;

        List<GameObject> tempNormalRooms = new List<GameObject>();

        //실제 플레이 가능 한 방을 생성
        foreach (var room in roomTypes)
        {
            // 남은 임시 일반 방이 없다면
            if (tempNormalRooms.Count <= 0) 
            {
                // 다시 모든 일반방 리스트를 받아온다.
                tempNormalRooms = normalRoomPrefaps.ToList();
            }

            // 방 타입을 기반으로 Prefap 배치
            switch (room.Value)
            {
                case RoomType.START:
                    prefab = specialRoomPrefaps[0];
                    break;
                case RoomType.NORMAL:
                    // 일반 방의 경우에는 방이 겹치지 않게 랜덤하게 방을 뽑아 배치하고 리스트에서 제외한다.
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
            // 각방의 위치는 (0,0)부터 시작 되어 180의 여백을 두고 생성
            Vector3 roomPos = new Vector3((room.Key.x - mapWidth / 2) * 180, (room.Key.y - mapHeight / 2) * 180, 0);
            // 방 생성
            GameObject tempRoom = Instantiate(prefab,roomPos,Quaternion.identity);
            Vector2Int pos = room.Key;
            // 방의 좌표는 방 스크립트로 전달한다.
            roomDic.Add(pos, tempRoom.GetComponent<Room>());
            // 방 초기화
            roomDic[pos].IntializeRoomData(pos, roomDoors[pos]);
            
            // 방이름은 좌표 값으로
            tempRoom.name = $"Room ({room.Key.x}, {room.Key.y})";
        }
    }

    public void SetMoveRoomDestination(Vector2Int currentPos, Vector2Int destination)
    {
        // 방 이동시 암전 효과(UI Manager)
        uiManager.FadeInMoveRoom(() =>
        {
            // FadeInMoveRoom이 끝난 후에 다음 코드 실행
            roomDic[destination].SetMoveSpawn(currentPos, destination);
            uiManager.RevealedWorldmap(destination);
        });
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
        //Debug.Log("미니맵 타일들이 갱신되었습니다.");
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