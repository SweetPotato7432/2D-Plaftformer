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

    [SerializeField] private int roomAmount;    // ������ ���� ����
    [SerializeField] private int mapWidth;      // ���� ���� ũ��
    [SerializeField] private int mapHeight;     // ���� ���� ũ��

    //private GameObject[,] roomArray;                 // ��Prefap���� ������ �迭(����׿�)
    // roomDoors�� Ű : ���� ��ǥ��, ��� : �� �� ���� ����
    // ����� �� ��ǥ ���ϴ� �� : Ű(���� ��ǥ ��) + ���(�� �� ����)
    private Dictionary<Vector2Int, List<Vector2Int>> roomDoors = new(); // �� ���� �� ����
    private Dictionary<Vector2Int, RoomType> roomTypes = new();
    private Dictionary<Vector2Int, int> roomDistances = new(); // �� ����� ���� �����κ��� �󸶳� ������ �ִ���
    private List<Vector2Int> createdRooms = new();   // ������ ����� ����Ʈ
    private HashSet<Vector2Int> blockedPositions = new(); // ������ �� ��ġ
    private List<Vector2Int> endRooms = new(); // ���� ��ġ

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

            // ���� �� �� �ʱ�ȭ �� �����
            //Debug.LogWarning($"Failed to generate enough rooms. Retrying ...");
        }

        // ������ �� �ľ� �� ����� ���� ���� �� �ı�
        
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
            Debug.Log($"Ű : {room.Key}, ��� : {room.Value}");
        }
        // �� ���� ����
        GeneratePlayableRoom();

        // �� �̴ϸ� ����
        Tilemap[] sourceTilemaps = FindSourceTilemapsWithLayer("Ground");
        GenerateMiniMap(sourceTilemaps);

        uiManager.GenerateWorldmap(roomTypes);
    }

    void GenerateMap()
    {
        Vector2Int startPosition = new Vector2Int(mapWidth / 2, mapHeight / 2);
        CreateRoom(startPosition);
        roomTypes[startPosition] = RoomType.START;

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
        // Instantiate �κ��� ���Ŀ� ����(������)
        //GameObject room = Instantiate(roomPrefab, new Vector3(position.x - mapWidth / 2, position.y - mapHeight / 2, 0), Quaternion.identity);
        //room.name = $"Room ({position.x}, {position.y})";
        //roomArray[position.x, position.y] = room;

        int distance = 0;

        roomTypes.Add(position, RoomType.NORMAL);
        createdRooms.Add(position);


        List<Vector2Int> doors = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        
        // �� 
        foreach (var direction in directions)
        {
            Vector2Int adjacentPos = position + direction;
            if (createdRooms.Contains(adjacentPos)) // ���⿡ ���� �����Ѵٸ�
            {
                
                doors.Add(direction);
                // ����� �� ���� ����
                if (IsValidRoomPosition(adjacentPos))
                {
                    if (!roomDoors[adjacentPos].Contains(-direction))
                    {
                        roomDoors[adjacentPos].Add(-direction);
                    }
                }
                // ���۹� ���� ������ �Ÿ� ����
                if (roomDistances[adjacentPos] >= distance)
                {
                    distance = roomDistances[adjacentPos]+1;
                }
            }
            else if (!blockedPositions.Contains(adjacentPos) && Random.Range(0f, 1f) < 0.5f) // ���� �������� Ȯ��, 50%�� Ȯ���� ����
            {
                doors.Add(direction);

                // ����� �� ������ ���Ӱ� ���� �濡���� �õ��Ѵ�. ��� �濡�� ������ ���ִ� ���� ����, �� �濡�� �� ����
            }
            else
            {
                blockedPositions.Add(adjacentPos);
            }
        }

        roomDoors[position] = doors;
        roomDistances[position] = distance;
        
    }

    // �۶߸��� �ִ� �� �������� ���� ����
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

    // ���� ũ�⸦ ������� Ȯ��
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
                if (!IsValidRoomPosition(adjacentPosition) || !createdRooms.Contains(adjacentPosition)/*roomArray[adjacentPosition.x, adjacentPosition.y] == null*/)
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

    // ���� ã��
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
            // ���� �濡 ������ �����, ��ó�� ���� �ϳ�������.
            if (blockedPositions.Contains(adjacentPosition)&& ClosedRoomCnt(adjacentPosition)==1)
            {
                CreateRoom(adjacentPosition);
                Debug.Log($"���� ���� ���� : {adjacentPosition}");
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
        // ���� �� �� ������, ������ ��� �������� ������, ����

        List<Vector2Int> tempEndRooms = endRooms.ToList();

        Dictionary<Vector2Int, int> endRoomDistance = new Dictionary<Vector2Int, int>();
        foreach (var room in tempEndRooms)
        {
            int distance = roomDistances[room];

            endRoomDistance.Add(room, distance);
        }

        var furthestRoom = endRoomDistance.Aggregate((maxRoom, nextRoom) => nextRoom.Value > maxRoom.Value ? nextRoom : maxRoom).Key;
        roomTypes[furthestRoom] = RoomType.BOSS;
        Debug.Log($"���� : {furthestRoom}");
        tempEndRooms.Remove(furthestRoom);

        int random = Random.Range(0, tempEndRooms.Count);
        roomTypes[tempEndRooms[random]] = RoomType.TREASURE;
        Debug.Log($"������ : {tempEndRooms[random]}");
        tempEndRooms.Remove(tempEndRooms[random]);

        random = Random.Range(0, tempEndRooms.Count);
        roomTypes[tempEndRooms[random]] = RoomType.SHOP;
        Debug.Log($"���� : {tempEndRooms[random]}");
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

        // ������ �ʱ�ȭ
        createdRooms.Clear();
        roomDoors.Clear();
        blockedPositions.Clear();
        roomTypes.Clear();


    }

    private void GeneratePlayableRoom()
    {
        GameObject prefab = null;

        List<GameObject> tempNormalRooms = new List<GameObject>();

        //���� �÷��� ���� �� ���� ����
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


    // Ground �±װ� ���� ��� Tilemap �ڵ� �˻�
    Tilemap[] FindSourceTilemapsWithLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogWarning($"���̾� \"{layerName}\" �� ã�� �� �����ϴ�.");
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

                    Vector3 worldPos = source.CellToWorld(localPos); // Ÿ���� ���� ���� ��ġ
                    Vector3Int miniMapCellPos = miniMapTilemap.WorldToCell(worldPos); // �̴ϸ� Ÿ�ϸ� ��ǥ�� ��ȯ

                    // �ߺ� üũ �� ��ġ
                    if (!miniMapTilemap.HasTile(miniMapCellPos))
                    {
                        miniMapTilemap.SetTile(miniMapCellPos, whiteTile);
                    }
                }
            }
        }

        Debug.Log("�̴ϸ� Ÿ�ϵ��� ���� ��ġ�� ���� ��ġ�Ǿ����ϴ�!");
    }

    public void GenerateMiniMapforTilemap(Tilemap[] sourceTilemaps)
    {
        int layer = LayerMask.NameToLayer("Ground");
        if (layer == -1)
        {
            Debug.LogWarning($"���̾� \"{"Ground"}\" �� ã�� �� �����ϴ�.");
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

                    Vector3 worldPos = source.CellToWorld(localPos); // Ÿ���� ���� ���� ��ġ
                    Vector3Int miniMapCellPos = miniMapTilemap.WorldToCell(worldPos); // �̴ϸ� Ÿ�ϸ� ��ǥ�� ��ȯ

                    // �ߺ� üũ �� ��ġ
                    if (!miniMapTilemap.HasTile(miniMapCellPos))
                    {
                        miniMapTilemap.SetTile(miniMapCellPos, whiteTile);
                    }
                }
            }
        }
        Debug.Log("�̴ϸ� Ÿ�ϵ��� ���ŵǾ����ϴ�.");
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

                miniMapTilemap.SetTile(miniMapCellPos, null); // Ÿ�� ����
            }
        }
    }
}