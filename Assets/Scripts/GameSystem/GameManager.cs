using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private Tilemap _tentativeMap;
    [SerializeField] private StageGenerator _stageGenerator;
    [SerializeField] private BlockManager _blockManager;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private Transform _effectParent;
    [SerializeField] private SubmitString _submitString;

    [SerializeField] private GameObject ExplsionPrefab;

    public Player _player { get; private set; }
    public Goal _goal { get; private set; }
    private int _deliveryBoxNum;
    private int _submittedDeliveryBoxNum;

    private bool isTimeStop;
    public bool IsTimeStop
    {
        get { return isTimeStop; }
        set { this.isTimeStop = value; }
    }
    private bool isGameOver;

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
        Initialize(0);
    }

    //初期化処理
    public void Initialize(int num)
    {
        isTimeStop = true;
        Restart();
    }

    private void Restart()
    {
        //プレイヤーが存在するなら破壊する(2回目以降の処理)
        if (_player != null)
        {
            Destroy(_player.gameObject);
        }
        DeleteEffect();

        //ステージ作成
        var tuple = _stageGenerator.CreateStageFromTilemap(_tentativeMap);

        //ブロックマネージャーの初期化
        _blockManager.Initialize(tuple.Item1, tuple.Item2);

        //プレイヤー作成
        _player = _stageGenerator.Initialize();

        //ゴール作成
        _goal = _stageGenerator._goal;

        //配達バコの数を初期化
        _deliveryBoxNum = _stageGenerator.deliveryboxNum;
        _submittedDeliveryBoxNum = 0;

        //カメラ設定
        _cameraManager.Initialize(_player, new Vector2(tuple.Item1.GetLength(0), tuple.Item1.GetLength(1)));
    }

    //ゲームオーバー時の処理
    public void GameOver(DamageBox explosionObj)
    {
        StartCoroutine(GameOverEnumerator(explosionObj));
    }

    private IEnumerator GameOverEnumerator(DamageBox explosionObj)
    {
        IsTimeStop = true;
        isGameOver = true;
        BlockManager.DissapearTriangle();
        yield return StartCoroutine(DieAnimation(explosionObj));
        yield return SceneChangeManager.instance.StartCoroutine(SceneChangeManager.CloseCurtain());
        Restart();
        yield return SceneChangeManager.instance.StartCoroutine(SceneChangeManager.OpenCurtain());
        IsTimeStop = false;
        isGameOver = false;
    }

    //死亡アニメーション
    private IEnumerator DieAnimation(DamageBox explosionObj)
    {
        Destroy(explosionObj.gameObject);
        //プレイヤーのダメージアニメーション
        if (_player.transform.position.x < explosionObj.transform.position.x)
        {
            _player.DamageAnimation(false);
        }
        else
        {
            _player.DamageAnimation(true);
        }
        //カメラがプレイヤーを追いかけないようにする
        _cameraManager.SetTarget(null);
        //爆発アニメーションの再生
        Instantiate(ExplsionPrefab, _effectParent).transform.position = explosionObj.transform.position;
        yield return new WaitForSeconds(0.7f);
    }

    //エフェクト全削除
    private void DeleteEffect()
    {
        foreach(Transform childTF in _effectParent)
        {
            Destroy(childTF.gameObject);
        }
    }

    //配達バコの提出
    public void SubmitDeliverBox()
    {
        //ゴールの配達バコを表示
        _goal.GetDeliverBoxSprite().SetActive(true);

        //糸の表示
        _submitString.Initialize();
        //糸の位置設定
        Vector3 pos = Vector3.zero;
        pos.x = _goal.transform.localPosition.x;
        pos.y = _cameraManager.transform.localPosition.y + 12;
        _submitString.transform.localPosition = pos;

        //糸を下に垂らす
        _submitString.transform.DOLocalMove(new Vector3(pos.x, _goal.transform.localPosition.y + 6.8f, 0),0.3f)
        .SetDelay(0.5f).SetEase(Ease.OutSine).OnComplete(instance.HangDeliverWithString);

        //提出した配達バコの数を更新
        _submittedDeliveryBoxNum++;
        if (_submittedDeliveryBoxNum == _deliveryBoxNum)
        {
            //ゲームクリア
            //TODO: ゲームクリア関数の実装
            Debug.Log("GameClear");
        }
    }

    //糸を下まで伸ばし切った後の処理
    private void HangDeliverWithString()
    {
        //ゴールの配達バコを非表示
        _goal.GetDeliverBoxSprite().SetActive(false);
        //糸の先の配達バコを表示
        _submitString.AppearDeliverBox();

        //糸を上に引き上げる
        StartCoroutine(PullUpString());
    }

    //糸を上に引き上げる
    private IEnumerator PullUpString()
    {
        yield return new WaitForSeconds(0.2f);

        Vector3 pos = _submitString.transform.localPosition;
        //糸を上げる
        _submitString.transform.DOLocalMove(new Vector3(pos.x, _cameraManager.transform.localPosition.y+14, 0),0.3f)
        .SetEase(Ease.OutSine).OnComplete(instance.DissapearString);
    }

    //糸を上に上げ切った後の処理
    private void DissapearString()
    {
        _submitString.gameObject.SetActive(false);
    }
}
