using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class DatabaseConnector : MonoBehaviour
{
    public static DatabaseConnector Instance;

    [Header("Firebase RTDB")]
    [SerializeField] private string databaseUrl = "https://YOUR-PROJECT-ID-default-rtdb.firebaseio.com";
    [SerializeField] private string authToken = ""; // 必要なら入れる（ID Token等）

    private RestApiRequester _client;

    private List<ExtraMapData> _loadObjs = new List<ExtraMapData>();
    public List<ExtraMapData> GetLoadObjs() => _loadObjs;

    public bool IsLoadFinish { get; private set; } = true;
    public bool IsPushFinish { get; private set; } = true;

    private bool _isAlreadyLoad;
    public bool IsAlreadyLoad => _isAlreadyLoad;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); return;
        }

        _client = new RestApiRequester(databaseUrl, string.IsNullOrEmpty(authToken) ? null : authToken);
    }
    
    public static void PushMapInfo(string name, int[,] mapData)
    {
        Instance.PushMapInfoAsync(name, mapData).Forget();
    }

    private async UniTaskVoid PushMapInfoAsync(string name, int[,] mapData)
    {
        try
        {
            while (!IsLoadFinish) await UniTask.Yield();

            var newMapData = new int[576];
            for (int x = 0; x < 32; x++)
            for (int y = 0; y < 18; y++)
                newMapData[x + y * 32] = mapData[x, y];

            var dto = new ExtraMapDataDto
            {
                name = name,
                mapData = newMapData,
                playNum = 0,
                createdTimeUnixMs = ExtraMapData.ToUnixMs(DateTime.Now),
            };

            // JsonUtilityは配列を含むクラスならOK（Dictionaryは不可）
            var body = JsonUtility.ToJson(dto);

            IsPushFinish = false;
            
            var res = await _client.PostAsync("StageData", body);
            Debug.Log($"PushMapInfoAsync response: {res}");
            var parsed = MiniJson.Deserialize(res) as Dictionary<string, object>;
            var newId = parsed != null && parsed.TryGetValue("name", out var v) ? v as string : null;

            Debug.Log($"セーブ成功 id={newId}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"セーブ失敗: {ex}");
        }
        finally
        {
            IsPushFinish = true;
        }
    }
    
    public static void FetchList()
    {
        Instance.FetchListAsync().Forget();
    }

    private async UniTaskVoid FetchListAsync()
    {
        try
        {
            while (!IsPushFinish) await UniTask.Yield();
            if (!IsLoadFinish) return;

            _isAlreadyLoad = true;
            IsLoadFinish = false;
            _loadObjs.Clear();

            // StageData全件取得
            var (json, _) = await _client.GetAsync("StageData", wantETag: false);
            
            Debug.Log($"FetchListAsync response: {json}");

            // 空なら "null"
            if (string.IsNullOrEmpty(json) || json == "null")
            {
                IsLoadFinish = true;
                return;
            }

            var root = MiniJson.Deserialize(json) as Dictionary<string, object>;
            if (root == null)
            {
                IsLoadFinish = true;
                return;
            }

            foreach (var kv in root)
            {
                var id = kv.Key;
                var obj = kv.Value as Dictionary<string, object>;
                if (obj == null) continue;

                // mapData は List<object> で来るので int[] に変換
                int[] mapData = null;
                if (obj.TryGetValue("mapData", out var md) && md is List<object> list)
                {
                    mapData = new int[list.Count];
                    for (int i = 0; i < list.Count; i++)
                        mapData[i] = Convert.ToInt32((double)list[i]); // MiniJSONはnumberをdoubleで返す
                }
                else mapData = Array.Empty<int>();

                var mapName = obj.TryGetValue("name", out var n) ? (string)n : "";
                var playNum = obj.TryGetValue("playNum", out var pn) ? Convert.ToInt32(pn is double d ? d : pn) : 0;
                var createdMs = obj.TryGetValue("createdTimeUnixMs", out var ct) ? Convert.ToInt64(ct is double dd ? dd : ct) : 0;

                _loadObjs.Add(new ExtraMapData(
                    mapData,
                    mapName,
                    playNum,
                    ExtraMapData.FromUnixMs(createdMs),
                    id
                ));
            }

            Debug.Log($"ロード成功: {_loadObjs.Count}件");
        }
        catch (Exception ex)
        {
            Debug.LogError($"ロード失敗: {ex}");
        }
        finally
        {
            IsLoadFinish = true;
        }
    }
    
    public static void AddPlayerNum(string mapID)
    {
        Instance.AddPlayerNumAsync(mapID).Forget();
    }

    private async UniTaskVoid AddPlayerNumAsync(string mapID)
    {
        const int maxRetry = 5;

        for (int attempt = 1; attempt <= maxRetry; attempt++)
        {
            try
            {
                var (json, etag) = await _client.GetAsync($"StageData/{mapID}", wantETag: true);

                if (string.IsNullOrEmpty(json) || json == "null")
                {
                    Debug.LogWarning($"AddPlayerNum: not found id={mapID}");
                    return;
                }

                var obj = MiniJson.Deserialize(json) as Dictionary<string, object>;
                if (obj == null) return;

                var playNum = obj.TryGetValue("playNum", out var pn)
                    ? Convert.ToInt32(pn is double d ? d : pn)
                    : 0;

                playNum++;

                // PUTする内容（必要なフィールド全部を維持するのが安全）
                // ここでは取得したobjをベースに playNum だけ上書きしてPUT
                obj["playNum"] = playNum;

                var newJson = MiniJson.Serialize(obj);

                // If-Match で「取得時と同じETagなら更新」を保証
                await _client.PutAsync($"StageData/{mapID}", newJson, ifMatchEtag: etag);

                // 成功
                return;
            }
            catch (Exception ex)
            {
                // 412 Precondition Failed（etag不一致）等は競合なのでリトライ
                // UnityWebRequestのerror文字列だけだと判別しづらいので、とりあえずリトライする実装
                if (attempt == maxRetry)
                {
                    Debug.LogError($"AddPlayerNum failed (attempt={attempt}): {ex}");
                    return;
                }
                await UniTask.Delay(100 * attempt);
            }
        }
    }
}
