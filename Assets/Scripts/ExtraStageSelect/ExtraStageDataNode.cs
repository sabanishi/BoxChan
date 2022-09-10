using System;
using System.Collections;
using System.Collections.Generic;
using NCMB;
using UnityEngine;

public class ExtraStageDataNode
{
    private BlockEnum[,] mapData;//ステージの情報
    private string mapName;//ステージ名
    private int playNum;//遊ばれた回数
    private DateTime createTime;//作られた時間
    private string objectID;//固有のID

    public BlockEnum[,] MapData
    {
        get { return mapData; }
    }

    public string MapName
    {
        get { return mapName; }
    }
    public int PlayNum
    {
        get { return playNum; }
    }
    public DateTime CreateTime
    {
        get { return createTime; }
    }
    public string ObjectID
    {
        get { return objectID; }
    }

    //コンストラクタ
    public ExtraStageDataNode(NCMBObject ncmbObject)
    {
        CreateMapData((ArrayList)ncmbObject["mapData"]);
        mapName = (string)ncmbObject["name"];
        playNum = Convert.ToInt32(ncmbObject["playNum"]);
        createTime = (DateTime)ncmbObject["createTime"];
        objectID = (string)ncmbObject.ObjectId;
    }

    //ステージ情報を含む二次元配列を作成
    private void CreateMapData(ArrayList list)
    {
        int[] map = new int[576];
        for (int j = 0; j < list.Count; j++)
        {
            if (j < 576)
            {
                map[j] = Convert.ToInt32(list[j]);
            }
        }

        mapData = new BlockEnum[32, 18];
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 18; y++)
            {
                mapData[x, y] = (BlockEnum)Enum.ToObject(typeof(BlockEnum), map[x + y * 32]);
            }
        }
    }
}
