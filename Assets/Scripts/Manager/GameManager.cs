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
            DontDestroyOnLoad(gameObject); // ���� �ٲ� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // ���� �ν��Ͻ��� ������ ���� ������ ���� �ı�
        }
        monsterInfo = csvLoadManager.GetMonsterList();
        playerInfo = csvLoadManager.GetPlayerList();
    }

    public CSVLoadManager csvLoadManager;
    PlayerController playerController;

    public CameraFollow cameraFollow;

    public List<PlayerInfo> playerInfo;
    public List<MonsterInfo> monsterInfo;
    

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
        transformPos.z = -10;
        cameraFollow.gameObject.transform.position = transformPos;
        //������� �߰� 0.1f�� ����?
        
    }

    public PlayerInfo PlayerStatInitialize(int id)
    {
        return playerInfo[id - 100];
    }

    public MonsterInfo EnemeyStatInitialize(int id)
    {
        return monsterInfo[id - 200];
    }


}
