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


    Dictionary<Vector2Int, GameObject> worldmapGameObject = new Dictionary<Vector2Int, GameObject>();

    Dictionary<Vector2Int, bool> worldmapRevealed = new Dictionary<Vector2Int, bool>();
    Dictionary<Vector2Int, bool> worldmapExpolered = new Dictionary<Vector2Int, bool>();
    // 최근에 방문한 방
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

    /// <summary>
    /// WorldMap 생성
    /// </summary>
    /// <param name="createdRooms">key : 생성된 방 좌표, value : 생성된 방 타입</param>
    public void GenerateWorldmap(Dictionary<Vector2Int, RoomManager.RoomType> createdRooms)
    {
        //좌표에 맞는 월드맵 UI 생성
        foreach (var room in createdRooms)
        {
            // 각 방의 위치를 키값과 prefap의 크기에 맞춰 설정한다.
            Vector3 roomPos = new Vector3(
                (room.Key.x - 10) * (roomWidth),
                (room.Key.y - 10) * (roomHeight),
                0);
            // 위치에 방 생성
            GameObject tempRoom = Instantiate(worldMapPrefap, roomPos, Quaternion.identity);

            // 생성된 방 오브젝트 비활성화
            tempRoom.SetActive(false);

            // 방을 생성하고 공개 완료 Dictionary에 false로 등록
            worldmapRevealed.Add((room.Key), false);
            // 방을 생성하고 탐색 완료 Dictionary에 false로 등록
            worldmapExpolered.Add((room.Key),false);

            // 방을 Content안에 넣어줌
            tempRoom.transform.SetParent(worldMapScrollRect.content.gameObject.transform, false);

            // 방 이름은 좌표 값으로
            tempRoom.name = $"Room ({room.Key.x}, {room.Key.y})";

            // worldmapGameObject dictionary에 오브젝트 등록
            worldmapGameObject.Add(room.Key,tempRoom);
        }

        // 시작방 공개 메서드 실행
        RevealedWorldmap(new Vector2Int(10, 10));
    }

    /// <summary>
    /// 최대 월드맵 콘텐츠 사이즈 계산
    /// </summary>
    /// <param name="revealedRoom">Key : 방의 좌표, Value : 공개 여부</param>
    void ResizeWorldmapContent(Dictionary<Vector2Int, bool> revealedRoom)
    {
        // 공개 여부가 참인 값들만 필터링
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

    /// <summary>
    /// 월드맵의 중심으로 정렬하는 메서드
    /// </summary>
    /// <param name="worldmapGameObject">Key : 월드맵의 좌표, Value : 월드맵 게임 오브젝트</param>
    void RecenteringWorldMap(Dictionary<Vector2Int, GameObject> worldmapGameObject)
    {
        // 생성된 게임 오브젝트가 없다면 종료
        if (worldmapGameObject.Count == 0) return;

        // 공개 된 월드맵 오브젝트의 최대 최소 값
        float minX = 0;
        float maxX = 0;
        float minY = 0;
        float maxY = 0;

        // 생성되어 있는 모든 월드맵 오브젝트
        foreach (var room in worldmapGameObject)
        {
            // 공개 여부가 거짓일 경우 넘어간다.
            if (!worldmapRevealed[room.Key]) continue;
            // 현재 방의 위치를 받아와서
            Vector3 pos = room.Value.transform.localPosition;

            // 최소 최대에 대입
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        //Debug.Log($"minX : {minX}, maxX : {maxX}, minY : {minY}, maxY : {maxY}");

        // 활성화 된 오브젝트 들의 중심 계산
        Vector3 newWorldmapCenter = new Vector3(
            (Mathf.Abs(maxX) - Mathf.Abs(minX)) / 2.0f,
            (Mathf.Abs(maxY) - Mathf.Abs(minY)) / 2.0f,
            0
        );

        // 이미 오브젝트들이 중심에 정렬되어있다면 종료
        if (newWorldmapCenter == Vector3.zero) return;

        //Debug.Log($"{newWorldmapCenter}");

        // 중심만큼 모든 오브젝트 반대로 이동
        foreach (var room in worldmapGameObject)
        {
            //Debug.Log($"{room.Value.transform.localPosition} => {room.Value.transform.localPosition+ newWorldmapCenter}");

            room.Value.transform.localPosition -= newWorldmapCenter;
        }
    }

    /// <summary>
    /// 월드맵 방 공개 메서드
    /// </summary>
    /// <param name="currentRoomPos"></param>
    public void RevealedWorldmap(Vector2Int currentRoomPos)
    {
        // 방문한 방 값 등록
        currentRoom = currentRoomPos;
        worldmapExpolered[currentRoomPos] = true;
        worldmapRevealed[currentRoomPos] = true;

        // 방문한 방 근처의 방이 존재한다면 공개 값을 true로 변경
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
        // 공개값을 기반으로 콘텐츠 크기 조절 메서드 실행
        ResizeWorldmapContent(worldmapRevealed);

        // 콘텐츠 크기를 조절 후 이미 생성되어 있는 GameObject를 중앙으로 이동시킨다.
        RecenteringWorldMap(worldmapGameObject);

        // 모든 정렬이 끝났다면, 월드맵의 색을 다시 설정한다.(공개 여부, 현재방)
        RedrawWorldmap();
    }

    /// <summary>
    /// 월드맵 오브젝트를 공개, 현재 위치에 따라 색과, Alpha 값을 변경하는 메서드
    /// </summary>
    void RedrawWorldmap()
    {
        //모든 worldGameObject를 확인한다.
        foreach (var worldmap in worldmapGameObject)
        {
            // 방이 탐색되지 않았지만, 공개는 되었다면
            if (!worldmapExpolered[worldmap.Key] && worldmapRevealed[worldmap.Key])
            {
                // Alpha값을 투명하게 변경
                worldmap.Value.GetComponent<RawImage>().color = new Color(1, 1, 1, 0.2f);
                worldmap.Value.SetActive(true);
            }
            // 방이 탐색되었고, 공개도 되었다면
            if (worldmapExpolered[worldmap.Key] && worldmapRevealed[worldmap.Key])
            {
                // 원래색을 나타내게 한다.
                worldmap.Value.GetComponent<RawImage>().color = new Color(1, 1, 1, 1f);
                worldmap.Value.SetActive(true);
            }
            // 만약 현재 위치한방이라면
            if(worldmap.Key == currentRoom)
            {
                // 초록색으로 변경한다.
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
