using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Coffee.UIExtensions;

public class CellBtnResponse : MonoBehaviour {

    private Button cellClickEvent;

     void OnEnable()
    {
        GameManager.Instance.Logo.SetActive(false);
        cellClickEvent = transform.GetComponent<Button>();
        cellClickEvent.onClick.RemoveAllListeners();
        cellClickEvent.onClick.AddListener(delegate (){
            SmallPicConvertToGreatPic();
        });
        StartCoroutine(Delay()); //做一个小的延时器，不然第一次会由于协程问题无法加载到第一张图
    }

     IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);
        if (transform.GetSiblingIndex() == 0)
        {
            cellClickEvent.onClick.Invoke();
        }
    }
    void SmallPicConvertToGreatPic()
    {
       // Debug.Log(transform.name + " : " + transform.GetChild(0).name);
        GameManager.Instance.ShowPic.GetComponent<RawImage>().texture
            = transform.GetComponent<RawImage>().texture;
        StartCoroutine(LoadPic());
    }

    IEnumerator LoadPic()
    {
        GameManager.Instance.Logo.SetActive(true);
        WWW www = new WWW(transform.GetChild(0).name);
        yield return www;        
        GameManager.Instance.ShowPic.GetComponent<RawImage>().texture = www.texture;
        GameManager.Instance.Logo.SetActive(false);
    }
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            transform.GetComponent<UIEffect>().enabled = true;
        }
        else
        {
            transform.GetComponent<UIEffect>().enabled = false;
        }
    }
}
