using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    [SerializeField,Header("Primary Interface Elements") ]
    public UILoopList MainCanvasContent;
    public GameObject mainCanvas;

    [SerializeField, Header("Secondary Interface Elements")]
    public GameObject ShowCanvasContent;
    public GameObject ShowCanvas;
    public GameObject ShowPic;
    public GameObject Logo;
    [SerializeField, Header("Secondary Datas")]
    public Text BrandName;
    public Text Rent;
    public Text RoomArea;
    public Text Address;
    public Text SetCnt;
    public Text SetRemaind;
    public Text Requirement;
    public Text ApartmentType;
    public Text Appointment;
    public Text Contact;
    public Text Guarantee;

    [SerializeField, Header("Transition interface")]
    public GameObject LoadingCanvas;

    [SerializeField, Header("Reuse Elements")]
    public GameObject itemPrefabs; //Primary Interface Reuse Element
    public GameObject cellPrefabs; //Secondary Interface Reuse Element


    [SerializeField, Header("FacilityIcons")]
    public Sprite[] FacilityIcon;
    public string[] FacilityName = {"电视","冰箱","洗衣机","宽带","热水器","空调","床","天然气","衣柜","暖气"};

    public Dictionary<string, Sprite> dicIcon = new Dictionary<string, Sprite>();

    [HideInInspector]
    public List<GameObject> listCells;

    List<string[]> result;

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < FacilityIcon.Length; i++)
        {
            dicIcon.Add(FacilityName[i], FacilityIcon[i]);
        }
    }
    // Use this for initialization
    void Start()
    {
        string sql = "SELECT * FROM `test`";
        result = DataDBContorller.Select(sql);
        List<string[]> listData = new List<string[]>();
        for (int i = 0; i < result.Count - 50; i++)
        {
            listData.Add(result[i]);
        }
        MainCanvasContent.Data(listData);
    }

    private void OnSelectedEventHandler(UILoopItem item)
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnMainCanvas();
        }
    }

    public void ReturnMainCanvas()
    {
        mainCanvas.SetActive(true);
        ShowCanvas.SetActive(false);
        LoadingCanvas.SetActive(false);
        PoolManager.Instance.ResetPool();
    }
}