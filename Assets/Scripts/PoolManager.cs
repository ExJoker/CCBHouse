using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {

    public static PoolManager Instance;

    private List<GameObject> cellPool = new List<GameObject>();
    private List<GameObject> cellUsed = new List<GameObject>();

    private GameObject cellPrefab;
    private void Awake()
    {
        Instance = this;
        cellPrefab = Resources.Load("cell") as GameObject;
      
    }
    // Use this for initialization
    void Start () {
        AutoInstantiate(cellPool, cellPrefab, 10);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void AutoInstantiate(List<GameObject> pool,GameObject go, int num)
    {
        for (int i = 0; i < num; i++)
        {
            pool.Add(GameObject.Instantiate(go, Vector3.zero, go.transform.rotation,GameManager.Instance.ShowCanvasContent.transform));
            pool[pool.Count - 1].SetActive(false);
        }
    }
    public GameObject GetGameObject()
    {
        List<GameObject> pool = cellPool;
        List<GameObject> poolUsed = cellUsed;

        GameObject go = cellPrefab;

        if (pool!=null && pool.Count > 0)
        {
            return UseGameObject(ref pool, ref poolUsed);
        }
        else
        {
            pool.Add(GameObject.Instantiate(go,Vector3.zero,Quaternion.identity));
            return UseGameObject(ref pool ,ref poolUsed);
        }             
    }

    private GameObject UseGameObject(ref List<GameObject> pool, ref List<GameObject> poolUse)
    {
        cellUsed.Add(pool[0]);
        pool.Remove(pool[0]);
        return poolUse[poolUse.Count - 1];
    }

    public void ResetPool()
    {
        MergeList(ref cellPool,ref cellUsed);
    }


    void MergeList(ref List<GameObject> pool, ref List<GameObject> usePool)
    {
        //将未使用的对象放入 使用池中
        foreach (GameObject g in pool)
        {
            usePool.Add(g);
        }

        //对象池重新指向新对象
        pool = new List<GameObject>();

        //将使用的对象放入 新池中
        foreach (GameObject g in usePool)
        {
            pool.Add(g);
        }

        //使用池重新指向新对象
        usePool = new List<GameObject>();
        foreach (GameObject g in pool)
        {
            g.SetActive(false);
        }

    }
}
