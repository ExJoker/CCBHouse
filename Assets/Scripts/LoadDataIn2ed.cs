using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        StartCoroutine(DisposeIMGInSecondary());
    }

    IEnumerator DisposeIMGInSecondary()
    {
        string[] strs = result[0][4].ToString().Split(',');
        GameManager.Instance.ShowCanvasContent.GetComponent<RectTransform>().sizeDelta = new Vector2( (width + gaps.x) * strs.Length + gaps.x , height);        
        for (int i = 0; i < strs.Length; i++)
        {
            GameObject go = PoolManager.Instance.GetGameObject();
            go.transform.localPosition = new Vector3(i * (width + gaps.x) + width /2.0f + gaps.x, 0);
            WWW www = new WWW(strs[i]);
            yield return www;
            go.GetComponent<RawImage>().texture = www.texture;
            GameManager.Instance.listCells.Add(go);
            go.SetActive(true);
        }
        GameManager.Instance.LoadingCanvas.SetActive(false);
        GameManager.Instance.mainCanvas.SetActive(false);
    }

}
