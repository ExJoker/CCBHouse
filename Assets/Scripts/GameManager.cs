using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public UILoopList uiList;

    List<string[]> result;
    // Use this for initialization
    void Start()
    {
        string sql = "SELECT * FROM `brand`";
        result = DataDBContorller.Select(sql);
        uiList.onSelectedEvent = OnSelectedEventHandler;
        List<string[]> listData = new List<string[]>();
        for (int i = 0; i < result.Count-3; i++)
        {
            listData.Add(result[i]);
        }
        uiList.Data(listData);
    }

    private void OnSelectedEventHandler(UILoopItem item)
    {
        Debug.LogError("选择的单元数据为:" + item.GetData()[1].ToString());
    }

    // Update is called once per frame
    void Update()
    {

    }
}