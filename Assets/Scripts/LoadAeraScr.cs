using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadAeraScr : MonoBehaviour {

    public static LoadAeraScr Instance;

    List<string[]> result;

    private RectTransform areaBtn;
    private float width;
    private float height;
    private float gaps = 20.0f;



    private void Awake()
    {
        Instance = this;
    }



    // Use this for initialization
    void Start () {
        areaBtn = GameManager.Instance.areaBtn.GetComponent<RectTransform>();
        width = areaBtn.rect.width;
        height = areaBtn.rect.height;
        LoadAreaButtonMethod();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

   void LoadAreaButtonMethod()
    {
        string sql = "SELECT *FROM Region WHERE State = 1;";
        result = DataDBContorller.Select(sql);
        for (int i = 0; i < result.Count; i++)
        {
            GameObject go = GameObject.Instantiate(areaBtn.gameObject,GameManager.Instance.MainCanvasContentInAreaList.transform) as GameObject;
            go.transform.localPosition = new Vector3(i * width + width / 2.0f + gaps , - (height + gaps) / 2.0f  );
            go.transform.GetChild(0).GetComponent<Text>().text = result[i][1].ToString();
        }
    }

}
