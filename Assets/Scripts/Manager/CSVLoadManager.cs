using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerInfo
{
    public int id;
    public string characterName;
    public float hp;
    public bool attackType;
    public float attackRange;
    public float atk;
    public float attackSpeed;
    public float moveSpeed;
    public float maxJumpHeight;
    public float minJumpHeight;
    public float timeToJumpApex;
}

public class MonsterInfo
{
    public int id;
    public string characterName;
    public float hp;
    public bool attackType;
    public float attackRange;
    public float atk;
    public float attackSpeed;
    public float moveSpeed;
    public float maxJumpHeight;
    public float minJumpHeight;
    public float timeToJumpApex;
}

public class DropItemInfo
{
    public enum EffectType
    {
        Heal,
        Gold
    }

    public int id;
    public string itemName;
    public int rarity;
    public EffectType effectType;
    public int effectStatus;
    public string effect;
}

public class PassiveItemInfo
{
    public enum EffectType
    {
        Attack,
        Health,
        Speed
        
    }

    public int id;
    public string itemName;
    public int rarity;
    public EffectType effectType;
    public int effectStatus;
    public string effect;
}

public class CSVLoadManager : MonoBehaviour
{
    private List<List<string>> csvData = new List<List<string>>();

    private List<PlayerInfo> playerInfo = new List<PlayerInfo>();
    private List<MonsterInfo> monsterInfo = new List<MonsterInfo>();// 몬스터 정보
    private List<DropItemInfo> dropItemInfo = new List<DropItemInfo>();
    private List<PassiveItemInfo> passiveItemInfo = new List<PassiveItemInfo>();

    private void Awake()
    {
        LoadPlayerCSV();

        LoadMonsterCSV();

        LoadDropItemCSV();

        LoadPassiveItemCSV();
    }

    public List<PlayerInfo> GetPlayerList()
    {
        return playerInfo;
    }

    public List<MonsterInfo> GetMonsterList()
    {
        return monsterInfo;
    }

    public List<DropItemInfo> GetDropItemInfoList()
    {
        return dropItemInfo;
    }

    public List<PassiveItemInfo> GetPassiveItemInfoList()
    {

        return passiveItemInfo;
    }

    


    void LoadPlayerCSV() 
    {
        LoadCSV("Player", playerInfo, (row, info) =>
        {
            PlayerInfo player = info as PlayerInfo;
            if (player == null) return;

            int field_num = 0;
            foreach (string field in row)
            {
                //Debug.Log("field : " + field);
                switch (field_num)
                {
                    // 필요한 데이터 파싱 추가
                    case 0: player.id = int.Parse(field); break;
                    case 1: player.characterName = field; break;
                    case 2: player.hp = float.Parse(field); break;
                    case 3: player.attackType = Convert.ToBoolean(field); break;
                    case 4: player.attackRange = float.Parse(field); break;
                    case 5: player.atk = float.Parse(field); break;
                    case 6: player.attackSpeed = float.Parse(field); break;
                    case 7: player.moveSpeed = float.Parse(field); break;
                    case 8: player.maxJumpHeight = float.Parse(field); break;
                    case 9: player.minJumpHeight = float.Parse(field); break;
                    case 10: player.timeToJumpApex = float.Parse(field); break;
                }
                field_num++;
            }
        });
    }
    void LoadMonsterCSV()
    {
        LoadCSV("Monster", monsterInfo, (row, info) =>
        {
            MonsterInfo monster = info as MonsterInfo;
            if (monster == null) return;

            int field_num = 0;
            foreach (string field in row)
            {
                //Debug.Log("field : " + field);
                switch (field_num)
                {
                    // 필요한 데이터 파싱 추가
                    case 0: monster.id = int.Parse(field); break;
                    case 1: monster.characterName = field; break;
                    case 2: monster.hp = float.Parse(field);break;
                    case 3: monster.attackType = Convert.ToBoolean(field); break;
                    case 4: monster.attackRange = float.Parse(field); break;
                    case 5: monster.atk = float.Parse(field); break;
                    case 6: monster.attackSpeed = float.Parse(field); break;
                    case 7: monster.moveSpeed = float.Parse(field); break;
                    case 8: monster.maxJumpHeight = float.Parse(field); break;
                    case 9: monster.minJumpHeight = float.Parse(field); break;
                    case 10: monster.timeToJumpApex = float.Parse(field); break;
                }
                field_num++;
            }
        });
    }

    void LoadDropItemCSV()
    {
        LoadCSV("DropItem", dropItemInfo, (row, info) =>
        {
            DropItemInfo dropItemInfo = info as DropItemInfo;
            if (dropItemInfo == null) return;

            int field_num = 0;
            foreach (string field in row)
            {
                //Debug.Log("field : " + field);
                switch (field_num)
                {
                    // 필요한 데이터 파싱 추가
                    case 0: dropItemInfo.id = int.Parse(field); break;
                    case 1: dropItemInfo.itemName = field; break;
                    case 2: dropItemInfo.rarity = int.Parse(field); break;
                    case 3: dropItemInfo.effectType = (DropItemInfo.EffectType)Enum.Parse(typeof(DropItemInfo.EffectType),field); break;
                    case 4: dropItemInfo.effectStatus = int.Parse(field); break;
                    case 5: dropItemInfo.effect  = field; break;
                }
                field_num++;
            }
        });
    }

    void LoadPassiveItemCSV()
    {
        LoadCSV("PassiveItem", passiveItemInfo, (row, info) =>
        {
            PassiveItemInfo passiveItemInfo = info as PassiveItemInfo;
            if (passiveItemInfo == null) return;

            int field_num = 0;
            foreach (string field in row)
            {
                //Debug.Log("field : " + field);
                switch (field_num)
                {
                    // 필요한 데이터 파싱 추가
                    case 0: passiveItemInfo.id = int.Parse(field); break;
                    case 1: passiveItemInfo.itemName = field; break;
                    case 2: passiveItemInfo.rarity = int.Parse(field); break;
                    case 3: passiveItemInfo.effectType = (PassiveItemInfo.EffectType)Enum.Parse(typeof(PassiveItemInfo.EffectType), field); break;
                    case 4: passiveItemInfo.effectStatus = int.Parse(field); break;
                    case 5: passiveItemInfo.effect = field; break;
                }
                field_num++;
            }
        });
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadCSV<T>(string resourceName, List<T> dataList, Action<List<string>, T> processRow) where T : new()
    {
        csvData.Clear();

        TextAsset csvFile = Resources.Load<TextAsset>(resourceName);
        if (csvFile != null)
        {
            //Debug.Log($"{resourceName} 파일이 존재합니다.");
            string[] rows = csvFile.text.Split('\n');

            foreach (string row in rows)
            {
                string[] fields = row.Split(',');
                List<string> rowData = new List<string>(fields);
                csvData.Add(rowData);
            }

            int row_num = 0;
            foreach (List<string> row in csvData)
            {
                if (row_num == 0) // 첫 번째 행(헤더) 스킵
                {
                    row_num++;
                    continue;
                }

                //Debug.Log($"[{row_num}]");
                T info = new T();

                processRow(row, info); // 전달된 델리게이트 실행

                dataList.Add(info);
                row_num++;
            }
        }
        else
        {
            Debug.LogWarning($"{resourceName} 파일이 존재하지 않습니다.");
        }
    }
}
