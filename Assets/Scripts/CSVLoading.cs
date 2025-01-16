using UnityEngine;
using System.Collections.Generic;

public class CSVLoading
{
    //csv는 리스트 구조
    // string 리스트를 리스트로
    private List<List<string>> csvData = new List<List<string>>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        //// TextAsset : 
        //// csv파일을 불러오고 있을시
        //TextAsset csvFile = Resources.Load<TextAsset>("Example");
        //if(csvFile != null)
        //{
        //    Debug.Log("파일이 존재합니다.");
        //    // '\n' : 줄바꿈
        //    // 줄바꿈을 찾아서 분리한다.
        //    string[] rows = csvFile.text.Split('\n');

        //    foreach (string row in rows)
        //    {
        //        string[] fields = row.Split(',');
        //        List<string> rowData = new List<string>(fields);
        //        csvData.Add(rowData);
        //    }

        //    int row_num = 0;
        //    foreach(List<string> row in csvData)
        //    {
        //        Debug.Log("["+(row_num+1)+"]행");
        //        int field_num = 0;
        //        foreach(string field in row)
        //        {
        //            switch (field_num)
        //            {
        //                case 0:
        //                    Debug.Log($"1열 : {int.Parse(field)}");
        //                    break;
        //                case 1:
        //                    Debug.Log($"2열 : {int.Parse(field)}");
        //                    break;
        //                case 2:
        //                    Debug.Log($"3열 : {float.Parse(field)}");
        //                    break;
        //                case 3:
        //                    Debug.Log($"4열 : {float.Parse(field)}");
        //                    break;
        //            }
        //            field_num++;
        //        }
                
        //    }
            
        //}
        //else
        //{
        //    Debug.LogError("파일이 존재하지 않습니다.");
        //}


    }

    public List<List<string>> csvLoad(string csvName)
    {
        // TextAsset : 
        // csv파일을 불러오고 있을시
        TextAsset csvFile = Resources.Load<TextAsset>(csvName);
        if (csvFile != null)
        {
            Debug.Log("파일이 존재합니다.");
            // '\n' : 줄바꿈
            // 줄바꿈을 찾아서 분리한다.
            string[] rows = csvFile.text.Split('\n');

            foreach (string row in rows)
            {
                string[] fields = row.Split(',');
                List<string> rowData = new List<string>(fields);
                csvData.Add(rowData);
            }
        }
        else
        {
            Debug.LogError("파일이 존재하지 않습니다.");
        }

        return csvData;
    }



}
