using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 있으면 새로 생성된 것을 파괴
        }
        monsterInfo = csvLoadManager.GetMonsterList();
        playerInfo = csvLoadManager.GetPlayerList();
        dropItemInfo = csvLoadManager.GetDropItemInfoList();
    }

    public CSVLoadManager csvLoadManager;
    PlayerController playerController;

    public CameraFollow cameraFollow;

    public List<PlayerInfo> playerInfo;
    public List<MonsterInfo> monsterInfo;
    public List<DropItemInfo> dropItemInfo;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerMoveRoom(Vector3 transformPos, float roomWidth, float roomHeight, Vector2 centerPos)
    {
        playerController.transform.position = transformPos;
        //Debug.Log($"{roomWidth} {roomHeight} {centerPos}");
        cameraFollow.SetCameraArea(roomWidth, roomHeight, centerPos);
        
        //cameraFollow.transform.position = playerController.transform.position;
    }

    public PlayerInfo PlayerStatInitialize(int id)
    {
        return playerInfo[id-101];
    }

    public MonsterInfo EnemyStatInitialize(int id)
    {
        return monsterInfo[id-201];
    }

    public DropItemInfo DropItemInfoInitialize(int id)
    {
        return dropItemInfo[id-1];
    }

    public void GameOver()
    {

    }


}
