﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UILoopItem : MonoBehaviour
{
    [System.NonSerialized]
    public int itemIndex;
    [System.NonSerialized]
    public GameObject itemObject;
    private IList data;
    public int ItemID
    {
        get
        {
            if (data == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(this.data[0].ToString());
            }
        }
    }

    private void Start()
    {
        StartCoroutine(LoadTextureToMainTexture());     
    }

    public void UpdateItem(int index, GameObject item)
    {
        itemIndex = index;
        itemObject = item;
    }
    public virtual void Data(object data)
    {
        this.data = data as IList;
        transform.Find("Text").GetComponent<Text>().text = this.data[1].ToString();
    }
    public virtual IList GetData()
    {
        return data;
    }
    public virtual void SetSelected(bool selected)
    {

    }

    IEnumerator LoadTextureToMainTexture()
    {
        string str = this.data[3].ToString();
        WWW www = new WWW(str);
        yield return www;
        transform.GetComponent<RawImage>().texture = www.texture;
    }

    public void OnSelectedEventHandler()
    {
        GameManager.Instance.LoadingCanvas.SetActive(true);
        GameManager.Instance.ShowCanvas.SetActive(true);
        LoadDataIn2ed.Instance.DisposeSecondaryInterfaceInfo(this.ItemID);
    }

}