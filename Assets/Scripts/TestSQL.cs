using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSQL : MonoBehaviour {
   public static TestSQL Instance;

    public Text textStr;

    public string[] RecStr;
    public RawImage rawImg;


    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start () {
       // StartCoroutine(LoadTextureToMainTexture());
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
        RecStr = DataDBContorller.Select("SELECT * FROM `brand`;")[0] as string[];
        //for (int i = 0; i < RecStr.Length; i++)
        //{
        //    RecStr[i].Trim();
        //}
        textStr.text = RecStr[0] + "\n" + RecStr[1].Trim() + "\n" + RecStr[2] 
            + "\n"+RecStr[3] + "\n" + RecStr[4] + "\n" + RecStr[5] + "\n" + RecStr[6]
            + "\n" + RecStr[7] + "\n" + RecStr[8] + "\n" + RecStr[9] + "\n" + RecStr[10]
            + "\n" + RecStr[11] + "\n" + RecStr[12] + "\n" + RecStr[13] + "\n" + RecStr[14] 
            + "\n" + RecStr[15]
            ;
         Debug.Log("房源信息 ： " + textStr.text);
       // StartCoroutine(LoadTextureToMainTexture());
    }

    IEnumerator LoadTextureToMainTexture()
    {
        WWW www = new WWW(
             RecStr[3]
            );
        yield return www;
        rawImg.texture = www.texture;
    }
}
