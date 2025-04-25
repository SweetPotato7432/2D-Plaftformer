using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    


    [SerializeField]
    GameObject optionUI;
    [SerializeField]
    GameObject worldmapUI;

    // Ȱ��ȭ �� UI ����
    Stack<GameObject> activeUI;

    [Header("Fade")]
    [SerializeField]
    private CanvasGroup fade_IMG;
    float fadeDuration = 0.25f;

    [Header("WorldMap")]
    [SerializeField]
    ScrollRect worldMapScrollRect;
    [SerializeField]
    GameObject worldMapPrefap;
    private float worldMapPadding = 10;

    RectTransform prefabTransform;

    float roomWidth;
    float roomHeight;

    // ����� ��ǥ �ִ�,�ּ�
    int minX;
    int maxX;
    int minY;
    int maxY;

    Dictionary<Vector2Int, GameObject> worldmapGameObject = new Dictionary<Vector2Int, GameObject>();

    Dictionary<Vector2Int, bool> worldmapRevealed = new Dictionary<Vector2Int, bool>();
    Dictionary<Vector2Int, bool> worldmapExpolered = new Dictionary<Vector2Int, bool>();
    Vector2Int currentRoom;

    [SerializeField]
    GameObject gameOverUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activeUI = new Stack<GameObject>();

        RectTransform prefabTransform = worldMapPrefap.GetComponent<RectTransform>();

        roomWidth = prefabTransform.sizeDelta.x + worldMapPadding;
        roomHeight = prefabTransform.sizeDelta.y + worldMapPadding;
    }

    // Update is called once per frame
    void Update()
    {
    }

    // UI 
    public void ActiveOptionUI()
    {
        if (optionUI.activeSelf)
        {
            activeUI.Pop();
            optionUI.SetActive(false);
            if (activeUI.Count != 0)
            {
                activeUI.Peek().SetActive(true);
            }
            
        }
        else
        {
            if(activeUI.Count != 0)
            {
                activeUI.Pop().SetActive(false);
                activeUI.Peek().SetActive(true);
                //foreach(GameObject go in activeUI)
                //{
                //    go.SetActive(false);
                //}
            }
            else
            {
                optionUI.SetActive(true);
                activeUI.Push(optionUI);
            }

        }
    }

    public void ActiveWorldMapUI()
    {
        if (worldmapUI.activeSelf)
        {
            activeUI.Pop();
            worldmapUI.SetActive(false);
            if (activeUI.Count != 0)
            {
                activeUI.Peek().SetActive(true);
            }
        }
        else
        {
            if (activeUI.Count != 0)
            {
                foreach (GameObject go in activeUI)
                {
                    go.SetActive(false);
                }
            }
            worldmapUI.SetActive(true);
            activeUI.Push(worldmapUI);
        }
    }

    // WorldMap
    public void GenerateWorldmap(Dictionary<Vector2Int, RoomManager.RoomType> createdRooms)
    {
        // �� ��ǥ���� �ּ�/�ִ밪 ���ϱ�
        minX = createdRooms.Min(r => r.Key.x);
        maxX = createdRooms.Max(r => r.Key.x);
        minY = createdRooms.Min(r => r.Key.y);
        maxY = createdRooms.Max(r => r.Key.y);

        ResizeWorldmapContent(createdRooms);

        

        //���� �÷��� ���� �� ���� ����
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

            tempRoom.SetActive(false);

            worldmapRevealed.Add((room.Key), false);
            worldmapExpolered.Add((room.Key),false);

            tempRoom.transform.SetParent(worldMapScrollRect.content.gameObject.transform, false);

            tempRoom.name = $"Room ({room.Key.x}, {room.Key.y})";

            worldmapGameObject.Add(room.Key,tempRoom);
        }
        RecenteringWorldMap(worldmapGameObject);

        RevealedWorldmap(new Vector2Int(10, 10));
        

    }

    void ResizeWorldmapContent(Dictionary<Vector2Int, RoomManager.RoomType> createdRooms)
    {
        // Content ���� ũ�� ���
        float contentWidth = (maxX - minX + 1) * roomWidth;
        float contentHeight = (maxY - minY + 1) * roomHeight;

        worldMapScrollRect.content.sizeDelta = new Vector2(
            contentWidth
            , contentHeight);
    }

    void RecenteringWorldMap(Dictionary<Vector2Int, GameObject> worldmapGameObject)
    {
        if (worldmapGameObject.Count == 0) return;

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var room in worldmapGameObject.Values)
        {
            Vector3 pos = room.transform.localPosition;

            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        // �ٿ�� �ڽ� �߽� ���
        Vector3 center = new Vector3(
            (minX + maxX) / 2f,
            (minY + maxY) / 2f,
            0
        );

        // �߽ɸ�ŭ ��� ������Ʈ �ݴ�� �̵�
        foreach (var room in worldmapGameObject.Values)
        {
            room.transform.localPosition -= center;
        }
    }

    public void RevealedWorldmap(Vector2Int currentRoomPos)
    {
        currentRoom = currentRoomPos;
        worldmapExpolered[currentRoomPos] = true;
        worldmapRevealed[currentRoomPos] = true;
        if (worldmapRevealed.ContainsKey(new Vector2Int(currentRoomPos.x + 1, currentRoomPos.y)))
        {
            worldmapRevealed[new Vector2Int(currentRoomPos.x + 1, currentRoomPos.y)] = true;
        }
        if (worldmapRevealed.ContainsKey(new Vector2Int(currentRoomPos.x, currentRoomPos.y+1)))
        {
            worldmapRevealed[new Vector2Int(currentRoomPos.x, currentRoomPos.y+1)] = true;
        }
        if (worldmapRevealed.ContainsKey(new Vector2Int(currentRoomPos.x - 1, currentRoomPos.y)))
        {
            worldmapRevealed[new Vector2Int(currentRoomPos.x - 1, currentRoomPos.y)] = true;
        }
        if (worldmapRevealed.ContainsKey(new Vector2Int(currentRoomPos.x, currentRoomPos.y-1)))
        {
            worldmapRevealed[new Vector2Int(currentRoomPos.x, currentRoomPos.y-1)] = true;
        }
        RedrawWorldmap();
    }

    void RedrawWorldmap()
    {
        foreach (var worldmap in worldmapGameObject)
        {
            if (!worldmapExpolered[worldmap.Key] && worldmapRevealed[worldmap.Key])
            {
                worldmap.Value.GetComponent<RawImage>().color = new Color(1, 1, 1, 0.2f);
                worldmap.Value.SetActive(true);
            }
            if (worldmapExpolered[worldmap.Key] && worldmapRevealed[worldmap.Key])
            {
                worldmap.Value.GetComponent<RawImage>().color = new Color(1, 1, 1, 1f);
                worldmap.Value.SetActive(true);
            }
            if(worldmap.Key == currentRoom)
            {
                worldmap.Value.GetComponent<RawImage>().color = Color.green;
            }

        }
    }

    public void FadeInMoveRoom(Action onComplete)
    {
        fade_IMG.DOFade(1, fadeDuration)
            .SetUpdate(true)
            .OnStart(() =>
            {
                fade_IMG.blocksRaycasts = true;
                Time.timeScale = 0f;
            })
            .OnComplete(() =>
            {
                onComplete?.Invoke();
                FadeOutMoveRoom();
            });

    }

    public void FadeOutMoveRoom()
    {
        fade_IMG.DOFade(0, fadeDuration)
            .SetUpdate(true)
            .OnStart(() =>
            {
                fade_IMG.blocksRaycasts = false;
                Time.timeScale = 1f;
            })
            .OnComplete(() =>
            {
                
            });

    }


    public void GameOverUIActive()
    {
        gameOverUI.SetActive(true);
    }
}
