using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/*
0.ID  
1.BrandName
2.State
3.BrandUrl
4.ImgMain
5.ImgList
6.HouseType
7.Price
8.Region
9.Rent
10.RoomArea
11.HouseName
12.ApartmentType
13.Address
14.Practice
15.RentalType
16.SetCnt
17.SetRemaind
18.Requirement
19.Appointment
20.AppoDate
21.AppoTime
22.AppoOptionDate
23.UpperLimit
24.Guarantee
25.Contact
26.Support
27.Environment
28.Traffic
29.DATE
*/
/// </summary>
public class LoadDataIn2ed : MonoBehaviour {

    public static LoadDataIn2ed Instance;

    List<string[]> result;
    private GameObject cellPrefabs;
    private RectTransform parentContent;
    private float width;
    private float height;
    private Vector2 gaps = new Vector2(20.0f,5.0f);


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        this.cellPrefabs = GameManager.Instance.cellPrefabs;
        this.parentContent = GameManager.Instance.ShowCanvasContent.GetComponent<RectTransform>();
        this.width = cellPrefabs.GetComponent<RectTransform>().rect.width;
        this.height = cellPrefabs.GetComponent<RectTransform>().rect.height;
    }

    public void DisposeSecondaryInterfaceInfo(int id)
    {
        string sql = "SELECT * FROM `test` WHERE ID = " + id +";";
        result = DataDBContorller.Select(sql);
        DisposeDataSecondary();
        StartCoroutine(DisposeIMGInSecondary());
    }

    IEnumerator DisposeIMGInSecondary()
    {
        string[] strs = result[0][5].ToString().Split(',');
        GameManager.Instance.ShowCanvasContent.GetComponent<RectTransform>().sizeDelta = new Vector2( (width + gaps.x) * strs.Length + gaps.x , height);        
        for (int i = 0; i < strs.Length; i++)
        {
            GameObject go = PoolManager.Instance.GetGameObject();
            go.transform.localPosition = new Vector3(i * (width + gaps.x) + width /2.0f + gaps.x, 0);
            WWW www = new WWW(strs[i]+ "?x-oss-process=image/resize,m_fixed,w_100");
            yield return www;
            go.GetComponent<RawImage>().texture = www.texture;
            GameManager.Instance.listCells.Add(go);
            go.SetActive(true);
            go.transform.GetChild(0).name = strs[i];
        }
        GameManager.Instance.LoadingCanvas.SetActive(false);
        GameManager.Instance.mainCanvas.SetActive(false);
    }


    void DisposeDataSecondary()
    {
        GameManager.Instance.BrandName.text = result[0][1];
        GameManager.Instance.Rent.text = result[0][9];
        GameManager.Instance.RoomArea.text = result[0][10];
        GameManager.Instance.Address.text = result[0][13];
        GameManager.Instance.SetCnt.text = result[0][16];
        GameManager.Instance.SetRemaind.text = result[0][17];
        GameManager.Instance.Requirement.text = result[0][18].Split('：')[1];
       // string[] strs = result[0][18].Split(':');
        GameManager.Instance.ApartmentType.text = result[0][12];
        GameManager.Instance.Appointment.text = result[0][19].Split('：')[1];
        GameManager.Instance.Contact.text = result[0][25].Split('：')[1];
    }

}
