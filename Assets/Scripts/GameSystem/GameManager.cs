using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]private Tilemap _tentativeMap;
    [SerializeField]private StageGenerator _stageGenerator;
    [SerializeField] private BlockManager _blockManager;
    [SerializeField]private CameraManager _cameraManager;

    public Player _player { get; private set; }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Initialize();
    }

    //初期化処理
    public void Initialize()
    {
        //ステージ作成
        var tuple = _stageGenerator.CreateStageFromTilemap(_tentativeMap);

        //ブロックマネージャーの初期化
        _blockManager.Initialize(tuple.Item1,tuple.Item2);

        //プレイヤー作成
        _player = _stageGenerator.Initialize();

        //カメラ設定
        _cameraManager.Initialize(_player, new Vector2(tuple.Item1.GetLength(0), tuple.Item1.GetLength(1)));
    }
}
