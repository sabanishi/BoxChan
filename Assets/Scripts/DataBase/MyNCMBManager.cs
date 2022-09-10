using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using System;

public class MyNCMBManager : MonoBehaviour
{
    public static MyNCMBManager instance;

    private List<NCMBObject> loadObjs;
    public List<NCMBObject> GetLoadObjs()
    {
        return loadObjs;
    }
    public bool isLoadFinish { get; private set; }
    public bool isPushFinish { get; private set; }

    private bool isAlreadyLoad;
    public bool IsAlreadyLoad
    {
        get { return isAlreadyLoad; }
    }

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        isLoadFinish = true;
        isPushFinish = true;
    }

    public static void PushMapInfo(string name, int[,] mapData)
    {
        instance.StartCoroutine(PushDeal(name, mapData));
    }

    private static IEnumerator PushDeal(string name,int[,] mapData)
    {
        while (!instance.isLoadFinish)
        {
            yield return null;
        }

        int[] newMapData = new int[576];
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 18; y++)
            {
                newMapData[x + y * 32] = mapData[x, y];
            }
        }

        NCMBObject obj = new NCMBObject("StageData");

        obj["name"] = name;
        obj["mapData"] = newMapData;
        obj["playNum"] = 0;
        obj["createTime"] = DateTime.Now;

        instance.isPushFinish = false;

        obj.Save((NCMBException e) =>
        {
            if (e == null)
            {
                //成功時
                Debug.Log("セーブ成功");
                instance.isPushFinish = true;
            }
            else
            {
                //エラー時
                Debug.Log("セーブ失敗");
            }
        });
    }

    public static void AddPlayerNum(string mapID)
    {
        NCMBObject obj = new NCMBObject("StageData");
        obj.ObjectId = mapID;
        obj.FetchAsync((NCMBException e) =>
        {
            if (e == null)
            {
                obj["playNum"] = Convert.ToInt32(obj["playNum"]) + 1;
                obj.SaveAsync();
            }
            else
            {
                Debug.Log(e.Message);
            }
        });
    }

    public static void FetchList()
    {
        instance.StartCoroutine(FetchDeal());
    }

    private static IEnumerator FetchDeal()
    {
        while (!instance.isPushFinish)
        {
            yield return null;
        }
        if (!instance.isLoadFinish)
        {
            yield break;
        }

        instance.isAlreadyLoad = true;
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("StageData");
        if (instance.loadObjs == null)
        {
            instance.loadObjs = new List<NCMBObject>();
        }
        instance.loadObjs.Clear();
        instance.isLoadFinish = false;

        query.FindAsync((List<NCMBObject> _objList, NCMBException e) =>
        {
            if (e == null)
            {
                foreach (var obj in _objList)
                {
                    instance.loadObjs.Add(obj);
                }
                instance.isLoadFinish = true;
            }
            else
            {
                Debug.Log("ロード失敗");
            }
        });
    }
}
