using System.Collections.Generic;
using System;
using UnityEngine;

public class MonsterInfo
{
    public int id;
    public string characterName;
    public float hp;
    public bool attackType;
    public float attackRange;
    public float attackSpeed;
    public float moveSpeed;
    public float jumpForce;
}

public class CSVLoadManager : MonoBehaviour
{
    private List<List<string>> csvData = new List<List<string>>();

    private List<MonsterInfo> monsterInfo = new List<MonsterInfo>();// ���� ����

    private void Awake()
    {
        LoadMonsterCsv();
    }

    public List<MonsterInfo> GetMonsterList()
    {
        return monsterInfo;
    }

    void LoadMonsterCsv()
    {
        LoadCsv("Monster", monsterInfo, (row, info) =>
        {
            MonsterInfo monster = info as MonsterInfo;
            if (monster == null) return;

            int field_num = 0;
            foreach (string field in row)
            {
                //Debug.Log("field : " + field);
                switch (field_num)
                {
                    // �ʿ��� ������ �Ľ� �߰�
                    case 0: monster.id = int.Parse(field); break;
                    case 1: monster.characterName = field; break;
                    case 2: monster.hp = float.Parse(field);break;
                    case 3: monster.attackType = Convert.ToBoolean(field); break;
                    case 4: monster.attackRange = float.Parse(field); break;
                    case 5: monster.attackSpeed = float.Parse(field); break;
                    case 6: monster.jumpForce = float.Parse(field); break;

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

    void LoadCsv<T>(string resourceName, List<T> dataList, Action<List<string>, T> processRow) where T : new()
    {
        csvData.Clear();

        TextAsset csvFile = Resources.Load<TextAsset>(resourceName);
        if (csvFile != null)
        {
            Debug.Log($"{resourceName} ������ �����մϴ�.");
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
                if (row_num == 0) // ù ��° ��(���) ��ŵ
                {
                    row_num++;
                    continue;
                }

                //Debug.Log($"[{row_num}]");
                T info = new T();

                processRow(row, info); // ���޵� ��������Ʈ ����

                dataList.Add(info);
                row_num++;
            }
        }
        else
        {
            Debug.Log($"{resourceName} ������ �������� �ʽ��ϴ�.");
        }
    }
}
