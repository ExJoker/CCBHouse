using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UILoopItem : MonoBehaviour
{

    [System.NonSerialized]
    public int itemIndex;
    [System.NonSerialized]
    public GameObject itemObject;
    private IList data;

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
        WWW www = new WWW("http://ccb-prod-oss.inboyu.com/upload/39eb11d5-5c0e-4cba-7e9c-d56ab48a78ee.jpg"
            // RecStr[3]
            );
        yield return www;
        transform.GetComponent<RawImage>().texture = www.texture;
    }
}