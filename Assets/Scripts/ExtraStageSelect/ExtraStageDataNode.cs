using System;

public class ExtraStageDataNode
{
    private BlockEnum[,] mapData;//ステージの情報
    private string mapName;//ステージ名
    private int playNum;//遊ばれた回数
    private DateTime createdTime;//作られた時間
    private string id;//固有のID

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
    public DateTime CreatedTime
    {
        get { return createdTime; }
    }
    public string ID
    {
        get { return id; }
    }

    //コンストラクタ
    public ExtraStageDataNode(ExtraMapData data)
    {
        CreateMapData(data.MapData);
        this.mapName = data.MapName;
        this.playNum = data.PlayNum;
        this.createdTime = data.CreatedTime;
        this.id = data.ID;
    }

    //ステージ情報を含む二次元配列を作成
    private void CreateMapData(int[] list)
    {
        if(list.Length != 576)
        {
            throw new ArgumentException("mapDataの長さが不正です。");
        }
        
        mapData = new BlockEnum[32, 18];
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 18; y++)
            {
                mapData[x, y] = (BlockEnum)Enum.ToObject(typeof(BlockEnum), list[x + y * 32]);
            }
        }
    }
}
