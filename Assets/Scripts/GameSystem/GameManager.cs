using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private Camera MainCamera;//カメラ
    [SerializeField] private StageGenerator _stageGenerator;//ステージ生成機構
    [SerializeField] private BlockManager _blockManager;//ハコを管理する機構
    [SerializeField] private CameraManager _cameraManager;//カメラマネージャー
    [SerializeField] private PauseManager _pauseManager;//ポーズ画面を管理するマネージャー
    [SerializeField] private Transform _effectParent;//全てのeffectの親オブジェクト
    [SerializeField] private SubmitString _submitString;//配達バコを提出した時の演出用のひも
    [SerializeField] private GameObject ExplsionPrefab;//危険なハコが爆発した時の演出用プレハブ
    [SerializeField] private MapStack mapStack;//全てのステージTilemapのリストを持っているオブジェクト

    //クリア画面用
    [SerializeField] [Header("クリア時の文字")] private GameObject[] ClearCharacterPrefabs;//クリア時に出てくる文字
    [SerializeField] private GameObject BlackBack;//クリア時の暗転用背景
    [SerializeField] private GameObject ClearTextInfo;//下から出てくる「クリア時間」とか白線とかが描いてある画像
    [SerializeField] private Text TimeCount;//クリア時間を記すためのText
    [SerializeField] private GameObject New1;//ゲームクリアが新記録の場合に表示するやつ
    [SerializeField] private GameObject PressSpace;//「スペースキーで戻る」と書かれた画像
    private List<GameObject> charas = new List<GameObject>();//C,L,E,A,Rを管理するためのリスト

    public Player _player { get; private set; }//プレイヤー
    public Goal _goal { get; private set; }//ゴール
    private int _deliveryBoxNum;//ステージ上(初期配置)にある配達バコの総数
    private int _submittedDeliveryBoxNum;//現在提出し終わった配達バコの数
    private Tilemap stageTileMap;//ステージ情報が記されたtileMap
    private bool canAcceptInput;//ゲームの入力を受け付けるかどうか
    private bool isClear;//クリア時の演出に入っているかどうか
    private float time;//現在のプレイ時間
    private string StageSceneName;//ステージの名前(Stage1- またはユーザーが作ったパズルの固有ID)
    private bool isGoBackFlag;//セレクト画面に戻れる状態に入ったかどうか
    private Sequence newSequence;//Newの文字用のDOTweenのSequence
    private bool isPause;//ポーズを管理するフラグ

    public bool CanAcceptInput
    {
        get { return canAcceptInput; }
        set { this.canAcceptInput = value; }
    }
    public bool IsClear()
    {
        return isClear;
    }
    public bool IsPause
    {
        get { return isPause; }
        set {
            isPause = value;
            _player.SetIsSimulate(!isPause);
            if (isPause)
            {
                DOTween.PauseAll();
            }
            else
            {
                DOTween.PlayAll();
            }
        }
    }

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
    }

    private void Update()
    {

        if (!isClear)
        {
            //時間を進める
            time += Time.deltaTime;

            //ポーズ画面の処理
            if (isPause)
            {
                _pauseManager.PauseUpdate();
            }
            else
            {
                if (Input.GetButtonDown("Pause")&&canAcceptInput)
                {
                    IsPause = true;
                    _pauseManager.StartCoroutine(_pauseManager.PauseStartCoroutine());
                }
            }
        }

        //クリア後にステージセレクト画面に戻る
        if (isGoBackFlag&&Input.GetButtonDown("Hang"))
        {
            SaveClearTime();//セーブ
            SceneChangeManager.GoSelect(SceneEnum.Game);//ステージセレクト画面の戻る
        }
    }

    //記録のセーブ
    private void SaveClearTime()
    {
        //もし今回の記録が以前の物より早ければ、記録を更新する
        if (time < SaveData.GetStageDataFromStagename(StageSceneName)
            ||SaveData.GetStageDataFromStagename(StageSceneName)==-1)
        {
            SaveData.SetStageData(StageSceneName, time);
        }
    }

    //初期化処理
    public void Initialize(string initialize_value)
    {
        StageSceneName = initialize_value;//ステージ名の受け渡し
        stageTileMap = mapStack.GetTileMap(initialize_value);//ステージのタイルマップを決定、代入する
        //TODO:エクストラパズルの場合の処理
        canAcceptInput = false;//入力を受け付けなくする
        time = 0;//ゲーム内時間の初期化
        isClear = false;//クリアフラグの初期化
        Restart();//再スタート処理
    }

    //ゲーム開始時、ゲームオーバー時、「はじめから」選択時に呼ばれる処理
    private void Restart()
    {
        //プレイヤーが存在するなら破壊する(2回目以降の処理)
        if (_player != null)
        {
            Destroy(_player.gameObject);
        }
        DeleteEffect();//エフェクトの破壊

        _blockManager.DeleteBlock();

        //クリア画面初期化
        foreach (var obj in charas)
        {
            Destroy(obj);
        }
        charas.Clear();//文字列削除
        ClearTextInfo.SetActive(false);
        BlackBack.SetActive(false);
        TimeCount.enabled = false;
        New1.SetActive(false);
        PressSpace.SetActive(false);
        isGoBackFlag = false;

        //ステージ作成
        var tuple = _stageGenerator.CreateStageFromTilemap(stageTileMap);

        //ブロックマネージャーの初期化
        _blockManager.Initialize(tuple.Item1, tuple.Item2);

        //プレイヤー作成
        _player = _stageGenerator.CreatePlayer();

        //ゴール作成
        _goal = _stageGenerator._goal;

        //配達バコの数を初期化
        _deliveryBoxNum = _stageGenerator.deliveryboxNum;
        _submittedDeliveryBoxNum = 0;

        //カメラ設定
        _cameraManager.Initialize(_player, new Vector2(tuple.Item1.GetLength(0), tuple.Item1.GetLength(1)));

        //ポーズ画面の初期化
        _pauseManager.PauseInitialize();
        IsPause = false;
    }

    //ゲームオーバー時の処理
    public void GameOver(DamageBox explosionObj)
    {
        StartCoroutine(GameOverCoroutine(explosionObj));
    }

    //ゲームオーバー時の一連の流れ
    private IEnumerator GameOverCoroutine(DamageBox explosionObj)
    {
        canAcceptInput = false;
        BlockManager.DissapearTriangle();
        yield return StartCoroutine(DieAnimationCoroutine(explosionObj));
        yield return StartCoroutine(RestartCoroutine());
    }

    //リスタート時の一連の流れ
    public IEnumerator RestartCoroutine()
    {
        canAcceptInput = false;
        yield return SceneChangeManager.instance.StartCoroutine(SceneChangeManager.CloseCurtainCoroutine());
        Restart();
        yield return SceneChangeManager.instance.StartCoroutine(SceneChangeManager.OpenCurtainCoroutine());
        canAcceptInput = true;
    }

    //死亡アニメーションの一連の流れ
    private IEnumerator DieAnimationCoroutine(DamageBox explosionObj)
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
        pos.y = _cameraManager.transform.localPosition.y + 17.5f;
        _submitString.transform.localPosition = pos;

        //糸を下に垂らす
        _submitString.transform.DOLocalMove(new Vector3(pos.x-0.02f, _goal.transform.localPosition.y + 8.75f, 0),0.5f)
        .SetDelay(0.5f).SetEase(Ease.OutSine).OnComplete(instance.HangDeliverWithString).SetLink(_submitString.gameObject);

        //提出した配達バコの数を更新
        _submittedDeliveryBoxNum++;
        if (_submittedDeliveryBoxNum == _deliveryBoxNum)
        {
            //ゲームクリア
            StartCoroutine(ClearDealCoroutine());
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
        StartCoroutine(PullUpStringCoroutine());
    }

    //糸を上に引き上げる処理
    private IEnumerator PullUpStringCoroutine()
    {
        if (isClear)
        {
            _player.transform.parent = _submitString.transform;
            _player.transform.localPosition = new Vector3(0.48f * _player.IsLeftThen1(), -6.0f,0);
        }
        yield return new WaitForSeconds(0.2f);

        Vector3 pos = _submitString.transform.localPosition;
        //糸を上げる
        _submitString.transform.DOLocalMove(new Vector3(pos.x, _cameraManager.transform.localPosition.y+19, 0),0.5f)
        .SetEase(Ease.OutSine).OnComplete(instance.DissapearString).SetLink(_submitString.gameObject);
    }

    //糸を上に上げ切った後の処理
    private void DissapearString()
    {
        _submitString.gameObject.SetActive(false);
    }

    //ゲームクリア時の処理の一連の流れ
    private IEnumerator ClearDealCoroutine()
    {
        //プレイヤーにクリアモーションを開始させる
        _player.ClearDeal(_goal.transform.localPosition,_submitString.gameObject);
        //カメラのクリアモーションを開始させる(ズーム)
        _cameraManager.StartClearZoom(new Vector3(_player.transform.localPosition.x,_player.transform.localPosition.y,-10));
        //クリアフラグを立てる
        isClear = true;

        yield return new WaitForSeconds(1.0f);

        //暗転
        BlackBack.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        //「CLEAR」の文字を表示
        for(int i = 0; i <= 4; i++)
        {
            CreateCharacter(ClearCharacterPrefabs[i]);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.25f);

        //白線などを下から飛び出させる
        ShowInfoText();
        yield return new WaitForSeconds(0.9f);
        //クリア時間の表示
        ShowTime();
        yield return new WaitForSeconds(0.1f);
        //「New!」の文字を表示させるべきなら表示させる
        ShowNew();
        yield return new WaitForSeconds(0.3f);
        //「スペースキーで戻る」を表示
        PressSpace.SetActive(true);
        isGoBackFlag = true;
    }

    //クリア時の「C L E A R」の文字の作成
    private void CreateCharacter(GameObject charaPrefab)
    {
        GameObject obj = Instantiate(charaPrefab, MainCamera.transform);
        obj.transform.localPosition = new Vector3(0, -3f, 10);
        charas.Add(obj);
    }

    //白線などを下から飛び出させる
    private void ShowInfoText()
    {
        ClearTextInfo.SetActive(true);
        ClearTextInfo.transform.localPosition = new Vector3(0, -5, 10);

        Hashtable moveHash = new Hashtable();
        moveHash.Add("position", new Vector3(0, 0.2f, 10));
        moveHash.Add("time", 0.5);
        moveHash.Add("delay", 0f);
        moveHash.Add("islocal", true);
        moveHash.Add("easeType", "easeOutQuart");
        iTween.MoveTo(ClearTextInfo, moveHash);
    }

    //クリア時間の表示
    private void ShowTime()
    {
        string timeText = Util.ConvertTimeFormat(time);
        TimeCount.text = timeText;
        TimeCount.enabled = true;
    }

    //「New!」の文字を表示させるべきなら表示させる
    private void ShowNew()
    {
        //クリア時間が従来の物より早ければ「New!」の文字を表示する
        if (time < SaveData.GetStageDataFromStagename(StageSceneName)
            || SaveData.GetStageDataFromStagename(StageSceneName) == -1)
        {
            New1.SetActive(true);
        }
    }

    //終了後の処理
    public void Terminate()
    {

    }
}
