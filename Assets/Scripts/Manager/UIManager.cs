using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("WorldMap")]
    [SerializeField]
    ScrollRect worldMapScrollRect;
    [SerializeField]
    GameObject worldMapPrefap;
    private float worldMapPadding = 10;

    RectTransform prefabTransform;
    RectTransform contentTransform;

    float roomWidth;
    float roomHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RectTransform prefabTransform = worldMapPrefap.GetComponent<RectTransform>();
        RectTransform contentTransform = worldMapScrollRect.content;

        roomWidth = prefabTransform.sizeDelta.x + worldMapPadding;
        roomHeight = prefabTransform.sizeDelta.y + worldMapPadding;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GenerateWorldmap(Dictionary<Vector2Int, RoomManager.RoomType> createdRooms)
    {
        //ResizeWorldmapContent(createdRooms);

        //실제 플레이 가능 한 방을 생성
        foreach (var room in createdRooms)
        {
            //switch (room.Value)
            //{
            //    case RoomType.START:
            //        worldMapPrefap = specialRoomPrefaps[0];
            //        break;
            //    case RoomType.NORMAL:
            //        int rand = Random.Range(0, tempNormalRooms.Count);

            //        worldMapPrefap = tempNormalRooms[rand];
            //        tempNormalRooms.RemoveAt(rand);
            //        //prefab = normalRoomPrefaps[1];
            //        break;
            //    case RoomType.TREASURE:
            //        worldMapPrefap = specialRoomPrefaps[1];
            //        break;
            //    case RoomType.SHOP:
            //        worldMapPrefap = specialRoomPrefaps[2];
            //        break;
            //    case RoomType.BOSS:
            //        worldMapPrefap = specialRoomPrefaps[3];
            //        break;
            //}

            Vector3 roomPos = new Vector3(
                (room.Key.x - 10) * (roomWidth),
                (room.Key.y - 10) * (roomHeight),
                0);
            GameObject tempRoom = Instantiate(worldMapPrefap, roomPos, Quaternion.identity);

            tempRoom.transform.SetParent(contentTransform, false);

            tempRoom.name = $"Room ({room.Key.x}, {room.Key.y})";
        }
    }

    void ResizeWorldmapContent(Dictionary<Vector2Int, RoomManager.RoomType> createdRooms)
    {
        // 방 좌표들의 최소/최대값 구하기
        int minX = createdRooms.Min(r => r.Key.x);
        int maxX = createdRooms.Max(r => r.Key.x);
        int minY = createdRooms.Min(r => r.Key.y);
        int maxY = createdRooms.Max(r => r.Key.y);

        // Content 영역 크기 계산
        float contentWidth = (maxX - minX + 1) * roomWidth;
        float contentHeight = (maxY - minY + 1) * roomHeight;

        contentTransform.sizeDelta = new Vector2(contentWidth, contentHeight);
    }
}
