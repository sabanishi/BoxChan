using System;

[Serializable]
public class ExtraMapDataDto
{
    public int[] mapData;
    public string name;
    public int playNum;
    public long createdTimeUnixMs;
}

public struct ExtraMapData
{
    public int[] MapData { get; }

    public string MapName { get; }

    public int PlayNum { get; }

    public DateTime CreatedTime { get; }

    public string ID { get; }
    
    public ExtraMapData(int[] mapData, string mapName, int playNum, DateTime createdTime, string id)
    {
        MapData = mapData;
        MapName = mapName;
        PlayNum = playNum;
        CreatedTime = createdTime;
        ID = id;
    }

    public static long ToUnixMs(DateTime dt)
    {
        return new DateTimeOffset(dt).ToUnixTimeMilliseconds();;
    }

    public static DateTime FromUnixMs(long ms)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime;
    }
}