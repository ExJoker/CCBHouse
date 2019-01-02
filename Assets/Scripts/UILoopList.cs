using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UILoopList : MonoBehaviour
{
    /// <summary>
    /// 滚动的方向
    /// </summary>
    enum Direction
    {
        Horizontal,//水平方向
        Vertical//垂直方向
    }
    /// <summary>
    /// 离上边的距离
    /// </summary>
    [SerializeField]
    private float topPadding = 0;
    /// <summary>
    /// 离下边的距离
    /// </summary>
    [SerializeField]
    private float bottomPadding = 0;
    [SerializeField]
    private RectTransform m_Cell;
    [SerializeField]
    private Vector2 m_Page = new Vector2(1, 1);//必须设置；几行几列
    [SerializeField]
    Direction direction = Direction.Horizontal;
    [SerializeField, Range(4, 10)]
    private int m_BufferNo;
    /// <summary>
    /// 间隔
    /// </summary>
    [SerializeField]
    private float cellGapX = 0f;
    [SerializeField]
    private float cellGapY = 0f;
    public delegate void OnSelectedEvent(UILoopItem item);
    /// <summary>
    /// 选择事件 但组件上一定要有Button组件 设置要在Data()设置数据前
    /// </summary>
    public OnSelectedEvent onSelectedEvent;//
    private List<RectTransform> m_InstantiateItems = new List<RectTransform>();
    private List<RectTransform> m_oldItems = new List<RectTransform>();
    private IList m_Datas;
    public Vector2 CellRect
    {
        get
        {
            return m_Cell != null ? new Vector2(cellGapX + m_Cell.sizeDelta.x, cellGapY + m_Cell.sizeDelta.y) : new Vector2(100, 100);
        }
    }
    public float CellScale { get { return direction == Direction.Horizontal ? CellRect.x : CellRect.y; } }
    private float m_PrevPos = 0;
    public float DirectionPos { get { return direction == Direction.Horizontal ? m_Rect.anchoredPosition.x : m_Rect.anchoredPosition.y; } }
    private int m_CurrentIndex;//页面的第一行（列）在整个conten中的位置
    private Vector2 m_InstantiateSize = Vector2.zero;
    public Vector2 InstantiateSize
    {
        get
        {
            if (m_InstantiateSize == Vector2.zero)
            {
                float rows, cols;
                if (direction == Direction.Horizontal)
                {
                    rows = m_Page.x;
                    cols = m_Page.y + (float)m_BufferNo;
                }
                else
                {
                    rows = m_Page.x + (float)m_BufferNo;
                    cols = m_Page.y;
                }
                m_InstantiateSize = new Vector2(rows, cols);
            }
            return m_InstantiateSize;
        }
    }
    public int PageCount { get { return (int)m_Page.x * (int)m_Page.y; } }
    public int PageScale { get { return direction == Direction.Horizontal ? (int)m_Page.x : (int)m_Page.y; } }
    private ScrollRect m_ScrollRect;
    private RectTransform m_Rect;
    public int InstantiateCount { get { return (int)InstantiateSize.x * (int)InstantiateSize.y; } }
    protected void Awake()
    {
        m_ScrollRect = GetComponentInParent<ScrollRect>();
        m_ScrollRect.horizontal = direction == Direction.Horizontal;
        m_ScrollRect.vertical = direction == Direction.Vertical;
        m_Rect = GetComponent<RectTransform>();
        if (m_Cell.transform.parent != null)
            m_Cell.gameObject.SetActive(false);
        m_Rect.anchorMax = Vector2.up;
        m_Rect.anchorMin = Vector2.up;
        m_Rect.pivot = Vector2.up;
        m_Rect.anchoredPosition = new Vector2(0f, 0f);
    }

    /// <summary>
    /// 设置数据 数据格式为IList
    /// </summary>
    /// <param name="data">Data.</param>
    public void Data(object data)
    {
        Reset();
        m_Datas = data as IList;
        if (m_Datas.Count > PageCount)
        {
            setBound(getRectByNum(m_Datas.Count));
        }
        else
        {
            setBound(m_Page);
        }
        if (m_Datas.Count > InstantiateCount)
        {
            while (m_InstantiateItems.Count < InstantiateCount)
            {
                createItem(m_InstantiateItems.Count);
            }
        }
        else
        {
            while (m_InstantiateItems.Count > m_Datas.Count)
            {
                removeItem(m_InstantiateItems.Count - 1);
            }
            while (m_InstantiateItems.Count < m_Datas.Count)
            {
                createItem(m_InstantiateItems.Count);
            }
        }
        if (m_Datas.Count > 0)
        {
            int count = Mathf.Min(m_InstantiateItems.Count, m_Datas.Count);
            for (int i = 0; i < count; i++)
            {
                updateItem(i, m_InstantiateItems[i].gameObject);
            }
        }
    }

    private void Reset()
    {

        for (int i = 0; i < m_InstantiateItems.Count; i++)
        {
            m_InstantiateItems[i].gameObject.SetActive(false);
            m_oldItems.Add(m_InstantiateItems[i]);
        }
        m_InstantiateItems.Clear();
        m_PrevPos = 0;
        m_CurrentIndex = 0;
        selectedObject = null;
        selectedItem = null;
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0);
    }
    public void SetIndexToBottom(int itemIndex)
    {
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, CellRect.y * itemIndex - m_ScrollRect.GetComponent<RectTransform>().sizeDelta.y + CellRect.y * 2 + topPadding + bottomPadding);
    }
    private void createItem(int index)
    {
        RectTransform item = null;
        if (m_oldItems.Count > 0)
        {
            item = m_oldItems[0];
            m_oldItems.Remove(item);
        }
        else
        {
            item = GameObject.Instantiate(m_Cell);
            item.SetParent(transform, false);
            item.anchorMax = Vector2.up;
            item.anchorMin = Vector2.up;
            item.pivot = Vector2.up;
        }

        //item.name = "item" + index;
        item.anchoredPosition = getPosByIndex(index);
        m_InstantiateItems.Add(item);
        item.gameObject.SetActive(true);
        //updateItem(index, item.gameObject);
    }
    private void removeItem(int index)
    {
        RectTransform item = m_InstantiateItems[index];
        m_InstantiateItems.Remove(item);
        item.gameObject.SetActive(false);
        m_oldItems.Add(item);
        //RectTransform.Destroy(item.gameObject);
    }
    /// <summary>
    /// 由格子数量获取多少行多少列
    /// </summary>
    /// <param name="num"></param>格子个数
    /// <returns></returns>
    private Vector2 getRectByNum(int num)
    {
        return direction == Direction.Horizontal ?
            new Vector2(m_Page.x, Mathf.CeilToInt(num / m_Page.x)) :
                new Vector2(Mathf.CeilToInt(num / m_Page.y), m_Page.y);
    }
    /// <summary>
    /// 设置content的大小
    /// </summary>
    /// <param name="rows"></param>行数
    /// <param name="cols"></param>列数
    private void setBound(Vector2 bound)
    {
        m_Rect.sizeDelta = new Vector2(bound.y * CellRect.x, bound.x * CellRect.y + bottomPadding + topPadding);
    }
    public float MaxPrevPos
    {
        get
        {
            float result;
            Vector2 max = getRectByNum(m_Datas.Count);
            if (direction == Direction.Horizontal)
            {
                result = max.y - m_Page.y;
            }
            else
            {
                result = max.x - m_Page.x;
            }
            return result * CellScale;
        }
    }
    public float scale { get { return direction == Direction.Horizontal ? 1f : -1f; } }
    private bool isFirst = true;
    void Update()
    {
        if (isFirst == true)
        {
            isFirst = false;
            return;
        }
        while (scale * DirectionPos - m_PrevPos < -CellScale * 2)
        {
            if (m_PrevPos <= -MaxPrevPos) return;
            m_PrevPos -= CellScale;
            List<RectTransform> range = m_InstantiateItems.GetRange(0, PageScale);
            m_InstantiateItems.RemoveRange(0, PageScale);
            m_InstantiateItems.AddRange(range);
            for (int i = 0; i < range.Count; i++)
            {
                moveItemToIndex(m_CurrentIndex * PageScale + m_InstantiateItems.Count + i, range[i]);
            }
            m_CurrentIndex++;
        }
        while (scale * DirectionPos - m_PrevPos > -CellScale)
        {
            if (Mathf.RoundToInt(m_PrevPos) >= 0) return;
            m_PrevPos += CellScale;
            m_CurrentIndex--;
            if (m_CurrentIndex < 0) return;
            List<RectTransform> range = m_InstantiateItems.GetRange(m_InstantiateItems.Count - PageScale, PageScale);
            m_InstantiateItems.RemoveRange(m_InstantiateItems.Count - PageScale, PageScale);
            m_InstantiateItems.InsertRange(0, range);
            for (int i = 0; i < range.Count; i++)
            {
                moveItemToIndex(m_CurrentIndex * PageScale + i, range[i]);
            }
        }
    }
    private void moveItemToIndex(int index, RectTransform item)
    {
        item.anchoredPosition = getPosByIndex(index);
        updateItem(index, item.gameObject);
    }
    private Vector2 getPosByIndex(int index)
    {
        return direction == Direction.Horizontal ?
            new Vector2(Mathf.Floor(index / InstantiateSize.x) * CellRect.x, -(index % InstantiateSize.x) * CellRect.y) :
                new Vector2((index % InstantiateSize.y) * CellRect.x, -Mathf.Floor(index / InstantiateSize.y) * CellRect.y - topPadding);
        //float x, y;
        //if(direction == Direction.Horizontal)
        //{
        //    x = index % m_Page.x;
        //    y = Mathf.FloorToInt(index / m_Page.x);
        //}
        //else
        //{
        //    x = Mathf.FloorToInt(index / m_Page.y);
        //    y = index % m_Page.y;
        //}
        //return new Vector2(y * CellRect.x, -x * CellRect.y);
    }
    private object selectedObject = null;
    [System.NonSerialized]
    public UILoopItem selectedItem = null;
    private void updateItem(int index, GameObject item)
    {
        item.SetActive(index < m_Datas.Count);
        if (item.activeSelf)
        {
            UILoopItem lit = item.GetComponent<UILoopItem>();
            lit.UpdateItem(index, item);
            lit.Data(m_Datas[index]);
            if (selectedObject == m_Datas[index])
            {
                lit.SetSelected(true);
            }
            else
            {
                lit.SetSelected(false);
            }
            if (lit.GetComponent<Button>() != null && onSelectedEvent != null && addClickEventList.IndexOf(lit.GetComponent<Button>()) < 0)
            {
                addClickEventList.Add(lit.GetComponent<Button>());
                lit.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    if (onSelectedEvent != null)
                    {
                        if (selectedItem != null && selectedItem != item.GetComponent<UILoopItem>())
                        {
                            selectedItem.SetSelected(false);
                        }
                        selectedItem = item.GetComponent<UILoopItem>();
                        selectedObject = selectedItem.GetData();
                        selectedItem.SetSelected(true);
                        onSelectedEvent(selectedItem);
                    }
                });
            }
        }
    }
    private List<Button> addClickEventList = new List<Button>();
    //void Start()
    //{
    //    List<int> a = new List<int>();
    //    for (int i = 0; i < 30; i++)
    //    {
    //        a.Add(i);
    //    }
    //    Data(a);
    //}
    /// <summary>
    /// 选中的对象
    /// </summary>
    /// <returns></returns>
    public object GetSelectedData()
    {
        return selectedObject;
    }
    /// <summary>
    /// 设置选中对象
    /// </summary>
    /// <param name="selectedIndex"></param>
    public void SetSelectedIndex(int selectedIndex)
    {
        if (m_Datas.Count > 0 && m_Datas.Count > selectedIndex)
        {
            selectedObject = m_Datas[selectedIndex];
            if (selectedItem != null)
            {
                selectedItem.SetSelected(false);
            }
            for (int i = 0; i < m_InstantiateItems.Count; i++)
            {
                if (selectedObject == m_InstantiateItems[i].GetComponent<UILoopItem>().GetData())
                {
                    m_InstantiateItems[i].GetComponent<UILoopItem>().SetSelected(true);
                    selectedItem = m_InstantiateItems[i].GetComponent<UILoopItem>();
                }
            }
        }
    }
}
