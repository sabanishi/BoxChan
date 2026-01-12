using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;

public class SaveData : MonoBehaviour
{
    public static SaveData instance;
    private static readonly string SaveKey = "stageData";

    public static readonly string[] STAGE_NAME_FOR_NORMAL_PUZZLE = new string[]
    {
        "","Stage1","Stage2","Stage3","Stage4","Stage5","Stage6","Stage7","Stage8",
        "Stage9","Stage10","Stage11","Stage12"
    };

    public static readonly int FINAL_STAGE_NUM=16;//ステージ数

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Dictionary<string,float> stageDataDict;

    //クリア時間を返す
    public static float GetStageDataFromStagename(string stageName)
    {
        if (instance.stageDataDict.ContainsKey(stageName))
        {
            return instance.stageDataDict[stageName];
        }
        else
        {
            //データが無い時、時間が-1になる
            return -1;
        }
    }

    //クリア時間と手数を追加する
    public static void SetStageData(string stageName,float stageData)
    {
        if (instance.stageDataDict.ContainsKey(stageName))
        {
            instance.stageDataDict[stageName] = stageData;
        }
        else
        {
            instance.stageDataDict.Add(stageName, stageData);
        }
        Save();
    }

    //データをセーブする
    private static void Save()
    {
        var json = JsonConvert.SerializeObject((instance.stageDataDict));
        PlayerPrefs.SetString(SaveKey,json);
    }

    //データをロードする
    public static void Load()
    {
        var json = PlayerPrefs.GetString(SaveKey,"null");
        if (json == "null")
        {
            instance.stageDataDict= new Dictionary<string,float>();
        }
        else
        {
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string,float>>(json);
            instance.stageDataDict = dictionary;
        }
    }

    //データを初期化する
    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
        instance.stageDataDict = new Dictionary<string, float>();
    }
}
