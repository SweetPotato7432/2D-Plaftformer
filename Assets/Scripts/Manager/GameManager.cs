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
            //DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 있으면 새로 생성된 것을 파괴
        }
        monsterInfo = csvLoadManager.GetMonsterList();
        playerInfo = csvLoadManager.GetPlayerList();
        dropItemInfo = csvLoadManager.GetDropItemInfoList();
        passiveItemInfo = csvLoadManager.GetPassiveItemInfoList();
    }

    public CSVLoadManager csvLoadManager;
    PlayerController playerController;

    public CameraFollow cameraFollow;

    public List<PlayerInfo> playerInfo;
    public List<MonsterInfo> monsterInfo;
    public List<DropItemInfo> dropItemInfo;
    public List<PassiveItemInfo> passiveItemInfo;

    // 드랍아이템의 희귀도 묶음
    public Dictionary<int, List<int>> dropItemRarityGroups = new Dictionary<int, List<int>>();
    public Dictionary<int, List<int>> passiveItemRarityGroups = new Dictionary<int, List<int>>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        InitializeDropItemRarityGroups();    
        InitializePassiveItemRarityGroups();

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

    public PassiveItemInfo PassiveItemInfoInitialize(int id)
    {
        return passiveItemInfo[id-1];
    }

    public int DropItemLength()
    {
        return dropItemInfo.Count;
    }

    public int PassiveItemLength()
    {
        return passiveItemInfo.Count;
    }

    public void InitializeDropItemRarityGroups()
    {
        dropItemRarityGroups.Clear();
        foreach(var meta in dropItemInfo)
        {
            if (!dropItemRarityGroups.ContainsKey(meta.rarity))
            {
                dropItemRarityGroups[meta.rarity] = new List<int>();
            }
            dropItemRarityGroups[meta.rarity].Add(meta.id);
        }
    }

    public void InitializePassiveItemRarityGroups()
    {
        passiveItemRarityGroups.Clear();
        foreach (var meta in passiveItemInfo)
        {
            if (!passiveItemRarityGroups.ContainsKey(meta.rarity))
            {
                passiveItemRarityGroups[meta.rarity] = new List<int>();
            }
            passiveItemRarityGroups[meta.rarity].Add(meta.id);
        }
    }



    public void GameOver()
    {

    }

    


}
