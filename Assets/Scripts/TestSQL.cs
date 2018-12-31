using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSQL : MonoBehaviour {

    public Text textStr;

    public string[] RecStr;
    public GameObject rawImg;

	// Use this for initialization
	void Start () {
        		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Select();
        }	
	}


    public void TestSql()
    {
        DataDBContorller.OpenSqlConnection(DataDBContorller.connectionString);
    }

    public void CloseSql()
    {
        DataDBContorller.CloseConnection();
    }

    public void Select()
    {
        RecStr = DataDBContorller.Select("SELECT * FROM `Data`;")[0] as string[];
        //for (int i = 0; i < RecStr.Length; i++)
        //{
        //    RecStr[i].Trim();
        //}
        textStr.text = RecStr[0] + "\n" + RecStr[1].Trim() + "\n" +
            RecStr[2] + "\n" + RecStr[4] + "元/月起\n" + RecStr[5] +"㎡"
            ;
        // Debug.Log(DataDBContorller.Select("SELECT * FROM `Data`;")[0]as string[]);
        StartCoroutine(LoadTextureToMainTexture());
    }

    IEnumerator LoadTextureToMainTexture()
    {
        WWW www = new WWW(RecStr[3]);
        yield return www;
        rawImg.GetComponent<RawImage>().texture = www.texture;
    }
}
