using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _Camera;
    [SerializeField] private CinemachineConfiner _CameraConfiner;
    [SerializeField] private float cameraDepth;
    [SerializeField] private PolygonCollider2D colliderPrefab;
    [SerializeField] private BoarderColliderObjects boaderColliers;

    private Player _player;

    public void Initialize(Player player,Vector2 screenSize)
    {
        _player = player;
        _Camera.Follow = player.transform;
        _Camera.m_Lens.OrthographicSize = cameraDepth;
        PolygonCollider2D limit = colliderPrefab;

        Vector2[] path = new Vector2[4];
        limit.pathCount = 1;
        path[0] = new Vector2(-0.5f, -0.5f);
        path[1] = new Vector2(-0.5f, screenSize.y+0.5f);
        path[2] = new Vector2(screenSize.x-0.5f, screenSize.y+0.5f);
        path[3] = new Vector2(screenSize.x-0.5f, -0.5f);

        limit.SetPath(0, path);
        SetLimit(limit);

        boaderColliers.SetBorderLine(screenSize.x, screenSize.y);
    }

    public void SetLimit(Collider2D limitCollider)
    {
        //カメラ制限区域を設定
        _CameraConfiner.m_BoundingShape2D = limitCollider;
    }

    public void SetTarget(Transform TF)
    {
        _Camera.Follow = TF;
    }

    public void SetOrthographsSize(float size)
    {
        _Camera.m_Lens.OrthographicSize = size;
    }
}
