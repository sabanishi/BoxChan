using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] public CinemachineVirtualCamera _Camera;
    [SerializeField] private CinemachineConfiner _CameraConfiner;//カメラの移動制限区域
    [SerializeField] private float cameraDepth;//カメラの深さ(どれくらいズームするか)
    [SerializeField] private PolygonCollider2D colliderPrefab;//カメラの移動を制限するためのcollider
    [SerializeField] private BoarderColliderObjects boaderColliers;//画面端からプレイヤーが落ちないようにするためのcollider
    [SerializeField] [Header("クリア時の深度")] private float clearOrthgraphSize = 2.5f;//クリア時のカメラの深さ

    private float clearCount;//クリア時の演出に使うカウンター

    public void Initialize(Player player,Vector2 screenSize)
    {
        //追従する対象の設定
        _Camera.Follow = player.transform;
        //カメラ深度の設定
        _Camera.m_Lens.OrthographicSize = cameraDepth;

        //カメラの移動制限区域の設定
        PolygonCollider2D limit = colliderPrefab;
        //移動制限区域がどこかを計算する
        Vector2[] path = new Vector2[4];
        limit.pathCount = 1;
        path[0] = new Vector2(-0.5f, -0.5f);
        path[1] = new Vector2(-0.5f, screenSize.y+0.5f);
        path[2] = new Vector2(screenSize.x-0.5f, screenSize.y+0.5f);
        path[3] = new Vector2(screenSize.x - 0.5f, -0.5f);
        limit.SetPath(0, path);
        SetLimit(limit);

        //画面端からプレイヤーがおちないようにするためのcolliderの作成
        boaderColliers.SetBorderLine(screenSize.x, screenSize.y);
    }

    void Update()
    {
        //クリア時のズーム処理
        if (clearCount > 0)
        {
            clearCount -= Time.deltaTime;
            if (clearCount > 0)
            {
                _Camera.m_Lens.OrthographicSize = clearOrthgraphSize + (cameraDepth - clearOrthgraphSize) * clearCount;
            }
        }
    }

    //クリア時のズームを開始する
    public void StartClearZoom(Vector3 goalPos)
    {
        clearCount = 0.5f;
        _Camera.transform.localPosition = goalPos;
        _Camera.Follow = null;
    }

    //カメラ制限区域を設定
    public void SetLimit(Collider2D limitCollider)
    {
        _CameraConfiner.m_BoundingShape2D = limitCollider;
    }

    //追従対象の設定
    public void SetTarget(Transform TF)
    {
        _Camera.Follow = TF;
    }

    //カメラ深度の設定
    public void SetOrthographsSize(float size)
    {
        _Camera.m_Lens.OrthographicSize = size;
    }
}
