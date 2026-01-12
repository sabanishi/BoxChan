using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class RestApiRequester
{
    private readonly string _dbUrl;
    private readonly string _authToken;

    public RestApiRequester(string databaseUrl, string authToken = null)
    {
        _dbUrl = databaseUrl?.TrimEnd('/');
        _authToken = authToken;
    }

    private string BuildUrl(string path)
    {
        var p = path.Trim('/');
        var url = $"{_dbUrl}/{p}.json";

        if (!string.IsNullOrEmpty(_authToken))
        {
            url += (url.Contains("?") ? "&" : "?") + "auth=" + UnityWebRequest.EscapeURL(_authToken);
        }
        return url;
    }

    public async UniTask<(string result, string etag)> GetAsync(string path, bool wantETag)
    {
        string result = "";
        string etag = null;
        
        using var req = UnityWebRequest.Get(BuildUrl(path));
        if (wantETag)
        {
            req.SetRequestHeader("X-Firebase-ETag", "true");
        }

        await req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
            throw new Exception($"GET failed: {req.responseCode} {req.error}\n{req.downloadHandler?.text}");

        if (wantETag)
        {
            etag = req.GetResponseHeader("ETag");
        }
        
        result = req.downloadHandler.text;

        return (result, etag);
    }

    public async UniTask<string> PostAsync(string path, string jsonBody)
    {
        using var req = new UnityWebRequest(BuildUrl(path), "POST");
        var bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json; charset=utf-8");

        await req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            throw new Exception($"POST failed: {req.responseCode} {req.error}\n{req.downloadHandler?.text}");
        }

        return req.downloadHandler.text;
    }

    public async UniTask PutAsync(string path, string jsonBody, string ifMatchEtag = null)
    {
        using var req = new UnityWebRequest(BuildUrl(path), "PUT");
        var bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json; charset=utf-8");

        if (!string.IsNullOrEmpty(ifMatchEtag))
        {
            req.SetRequestHeader("If-Match", ifMatchEtag);
        }

        await req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            throw new Exception($"PUT failed: {req.responseCode} {req.error}\n{req.downloadHandler?.text}");
        }
    }

    public async UniTask PatchAsync(string path, string jsonBody)
    {
        using var req = new UnityWebRequest(BuildUrl(path), "PATCH");
        var bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json; charset=utf-8");

        await req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            throw new Exception($"PATCH failed: {req.responseCode} {req.error}\n{req.downloadHandler?.text}");
        }
    }
}
