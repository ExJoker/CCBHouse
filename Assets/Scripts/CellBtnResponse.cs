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
        cellClickEvent = transform.GetComponent<Button>();
        cellClickEvent.onClick.RemoveAllListeners();
        cellClickEvent.onClick.AddListener(delegate () {
            SmallPicConvertToGreatPic();
        });
        if (transform.GetSiblingIndex() == 0)
        {
            cellClickEvent.onClick.Invoke();
        }
    }

    void SmallPicConvertToGreatPic()
    {
        GameManager.Instance.ShowPic.GetComponent<RawImage>().texture 
            = transform.GetComponent<RawImage>().texture;
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
