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
    }

    public CSVLoadManager csvLoadManager;
    PlayerController playerController;

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

    public void PlayerMoveRoom(Vector3 transformPos)
    {
        playerController.transform.position = transformPos;
    }

    public MonsterInfo EnemeyStatInitialize(int id)
    {
        return monsterInfo[id - 200];
    }
}
