using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    


    [SerializeField]
    GameObject optionUI;
    [SerializeField]
    GameObject worldmapUI;

    // 활성화 된 UI 저장
    Stack<GameObject> stackActiveUI;

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

    // 월드맵 좌표 최대,최소
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
        stackActiveUI = new Stack<GameObject>();

        RectTransform prefabTransform = worldMapPrefap.GetComponent<RectTransform>();


        roomWidth = prefabTransform.sizeDelta.x + worldMapPadding;
        roomHeight = prefabTransform.sizeDelta.y + worldMapPadding;


        UserInputManager.OnOptionInput -= OnOption;
        UserInputManager.OnOptionInput += OnOption;

        UserInputManager.OnWorldmapInput -= OnWorldmap;
        UserInputManager.OnWorldmapInput += OnWorldmap;
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    // 옵션창 활성화
    private void OnOption(bool isPressed)
    {
        if (isPressed)
        {
            // 활성화 된 UI가 있다면
            if (stackActiveUI.Count > 0)
            {
                // 가장 위에 활성화된 UI 비활성화
                stackActiveUI.Pop().SetActive(false);
                if(stackActiveUI.Count > 0)
                {
                    // 다음 UI 활성화
                    stackActiveUI.Peek().SetActive(true);
                }
            }
            // 활성화 된 UI가 없다면
            else
            {
                // 옵션 UI창 활성화
                optionUI.SetActive(true);
                stackActiveUI.Push(optionUI);

            }

        }

    }
    // 월드 맵 창 활성화
    private void OnWorldmap(bool isPressed)
    {
        if (isPressed)
        {
            // 활성화된 UI가 있다면
            if (stackActiveUI.Count > 0)
            {
                // 가장 위에 활성화 된 UI가 WorldmapUI가 아니라면 실행안함.
                if(stackActiveUI.Peek() != worldmapUI) return;

                // 가장 위에 활성화 된 UI가 WorldmapUI라면 비활성화
                stackActiveUI.Pop().SetActive(false);
                if (stackActiveUI.Count > 0)
                {
                    // 다음 UI 활성화
                    stackActiveUI.Peek().SetActive(true);
                }
            }
            // 활성화 된 UI가 없다면
            else
            {
                worldmapUI.SetActive(true);
                stackActiveUI.Push(worldmapUI);
            }
        }
    }

    // UI 
    public void ActiveWorldMapUI()
    {
        if (optionUI.activeSelf) return;

        if (worldmapUI.activeSelf)
        {
            stackActiveUI.Pop();
            worldmapUI.SetActive(false);
            if (stackActiveUI.Count != 0)
            {
                stackActiveUI.Peek().SetActive(true);
            }
        }
        else
        {
            if (stackActiveUI.Count != 0)
            {
                foreach (GameObject go in stackActiveUI)
                {
                    go.SetActive(false);
                }
            }
            worldmapUI.SetActive(true);
            stackActiveUI.Push(worldmapUI);
        }
    }

    // WorldMap 생성
    public void GenerateWorldmap(Dictionary<Vector2Int, RoomManager.RoomType> createdRooms)
    {
        // 방 좌표들의 최소/최대값 구하기
        minX = createdRooms.Min(r => r.Key.x);
        maxX = createdRooms.Max(r => r.Key.x);
        minY = createdRooms.Min(r => r.Key.y);
        maxY = createdRooms.Max(r => r.Key.y);



        //좌표에 맞는 월드맵 UI 생성
        foreach (var room in createdRooms)
        {
            Vector3 roomPos = new Vector3(
                (room.Key.x - 10) * (roomWidth),
                (room.Key.y - 10) * (roomHeight),
                0);
            GameObject tempRoom = Instantiate(worldMapPrefap, roomPos, Quaternion.identity);

            tempRoom.SetActive(false);

            // 방을 생성하고 공개, 탐색 완료 Dictionary에 등록
            worldmapRevealed.Add((room.Key), false);
            worldmapExpolered.Add((room.Key),false);

            // 방을 Content안에 넣어줌
            tempRoom.transform.SetParent(worldMapScrollRect.content.gameObject.transform, false);

            tempRoom.name = $"Room ({room.Key.x}, {room.Key.y})";

            worldmapGameObject.Add(room.Key,tempRoom);
        }

        ResizeWorldmapContent(worldmapRevealed);

        RevealedWorldmap(new Vector2Int(10, 10));
    }
    
    // 최대 월드맵 사이즈 계산
    void ResizeWorldmapContent(Dictionary<Vector2Int, bool> revealedRoom)
    {
        var filtered = revealedRoom.Where(r => r.Value);

        // 방 좌표들의 최소/최대값 구하기
        int revealMinX = filtered.Any() ? filtered.Min(r => r.Key.x) : 0;
        int revealMaxX = filtered.Any() ? filtered.Max(r => r.Key.x) : 0;
        int revealMinY = filtered.Any() ? filtered.Min(r => r.Key.y) : 0;
        int revealMaxY = filtered.Any() ? filtered.Max(r => r.Key.y) : 0;

        // Content 영역 크기 계산
        float contentWidth = (revealMaxX - revealMinX + 1) * roomWidth;
        float contentHeight = (revealMaxY - revealMinY + 1) * roomHeight;

        // 월드맵 콘텐츠의 사이즈를 설정
        worldMapScrollRect.content.sizeDelta = new Vector2(
            contentWidth
            , contentHeight);
    }

    void RecenteringWorldMap(Dictionary<Vector2Int, GameObject> worldmapGameObject)
    {
        if (worldmapGameObject.Count == 0) return;

        float minX = 0;
        float maxX = 0;
        float minY = 0;
        float maxY = 0;

        foreach (var room in worldmapGameObject.Values)
        {
            if(!room.gameObject.activeSelf) continue;
            Vector3 pos = room.transform.localPosition;

            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        // 바운딩 박스 중심 계산
        Vector3 center = new Vector3(
            (maxX - minX) / 2.0f,
            (maxY - minY) / 2.0f,
            0
        );

        // 중심만큼 모든 오브젝트 반대로 이동
        foreach (var room in worldmapGameObject)
        {
            room.Value.transform.localPosition = new Vector3(
                (room.Key.x - 10) * (roomWidth) - (center.x),
                (room.Key.y - 10) * (roomHeight) - (center.y),
                0);
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
        ResizeWorldmapContent(worldmapRevealed);

        RecenteringWorldMap(worldmapGameObject);

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

                StartCoroutine("FadeOutMoveRoom");
            });

    }
    
    IEnumerator FadeOutMoveRoom()
    {
        yield return new WaitForSecondsRealtime(.1f);

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

    private void OnDestroy()
    {

        UserInputManager.OnOptionInput -= OnOption;

        UserInputManager.OnWorldmapInput -= OnWorldmap;
    }
}
