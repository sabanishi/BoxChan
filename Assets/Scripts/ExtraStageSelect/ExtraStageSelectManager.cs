using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ExtraStageSelectManager : MonoBehaviour
{
    [SerializeField] private ExtraStageSelectScene parent;
    [SerializeField] private Dropdown dropDown;//新着順・人気順を切り替えるDropBox
    [SerializeField] private Letter letterPrefab;//手紙のプレハブ
    [SerializeField] private Transform lettersTF;//手紙の親となるオブジェクト
    [SerializeField] private LetterOrbitPos[] letterOrbitArray;//手紙を配置する場所のリスト
    [SerializeField] private Vector3 initialzieLetterPos;//ポストに入っている状態の手紙を生成する際の座標
    [SerializeField] private Vector3 initialzieLetterScale;//ポストに入っている状態の手紙を生成する際の大きさ
    [SerializeField] private Vector3 normalLetterScale;//選択可能な状態の手紙の大きさ
    [SerializeField] private IndexChangeButton downButton;//表示するステージの位置を戻す
    [SerializeField] private IndexChangeButton upButton;//表示するステージの位置を進める
    [SerializeField] private Text explainNowShowStageText;//現在どこを表示しているか説明するText
    [SerializeField] private GoBackButton goBackButton;//ステージセレクト画面に戻る画面
    [SerializeField] private SelectLetter selectLetter;//手紙を選択したときに使うオブジェクト
    [SerializeField] private GameObject blackBack;//半透明の黒背景

    private int indexNum;//現在表示しているマップの位置
    private List<ExtraStageDataNode> stageNodes = new List<ExtraStageDataNode>();//全てのステージ情報を記録したリスト
    private bool isFromStageSelect;//ステージセレクト画面から遷移してきたかどうか
    private bool isAnimationPlay;//アニメーション実行中かどうか
    private Letter selectLetterNode;//選択中の手紙
    private bool isSelect;
    private List<Letter> letters = new List<Letter>();

    public int IndexNum
    {
        set
        {
            indexNum = value;
            ExplainTextSetting();
            ButtonVisiableSetting();
        }
        get { return indexNum; }
    }

    [System.Serializable]
    public class LetterOrbitPos
    {
        public Vector3[] path;
    }

    public bool IsAnimationPlay
    {
        get { return isAnimationPlay; }
        set
        {
            isAnimationPlay = value;
            if (isAnimationPlay)
            {
                dropDown.enabled = false;
            }
            else
            {
                dropDown.enabled = true;
            }
        }
    }

    public bool IsSelect
    {
        get { return isSelect; }
    }

    //初期化処理
    public void Initialize(string initialize_value)
    {
        dropDown.value = 0;

        goBackButton.Initialize();
        goBackButton.gameObject.transform.localPosition = new Vector3(11, -3.8f, 0);

        if (initialize_value.Equals("StageSelect"))
        {
            isFromStageSelect = true;
            DiscardLetter();
            StartCoroutine(LoadStageData());
            selectLetter.Reset();
        }
        else
        {
            isFromStageSelect = false;
            parent.IsLoadFinish = true;
            blackBack.SetActive(false);
            isSelect = false;
            IsAnimationPlay = false;
            selectLetter.Reset();
            selectLetterNode.GetSpriteRenderer().enabled = true;
            selectLetterNode.GoStablePos(0.1f);
        }
    }

    //手紙などを初期化
    private void DiscardLetter()
    {
        foreach(Letter letter in letters)
        {
            Destroy(letter.gameObject);
        }
        letters.Clear();
    }

    //ステージの情報を読み込む
    private IEnumerator LoadStageData()
    {
        while (!DatabaseConnector.Instance.IsLoadFinish)
        {
            yield return null;
        }

        stageNodes.Clear();

        List<ExtraMapData> mapDataList = DatabaseConnector.Instance.GetLoadObjs();
        if (mapDataList != null && mapDataList.Count > 0)
        {
            foreach (ExtraMapData data in mapDataList)
            {
                stageNodes.Add(new ExtraStageDataNode(data));
            }
        }
        SortByDate();
        parent.IsLoadFinish = true;
        yield return null;
    }

    //カーテンが上がった後の処理
    public void AfterOpenCurtainDeal()
    {
        //「戻る」ボタンのアニメーション
        goBackButton.gameObject.transform.DOLocalMove(new Vector3(6.8f, -3.8f, 0), 0.5f)
            .OnComplete(goBackButton.SetIsValidTrue);

        if (isFromStageSelect)
        {
            IndexNum = 0;
        }
    }

    //ポストから最大6枚まで手紙を放出する
    private IEnumerator ReleaseLetters()
    {
        IsAnimationPlay = true;
        for(int i=indexNum*6; i < Mathf.Min(stageNodes.Count,(indexNum+1)*6); i++)
        {
            CreateLetter(i, i - indexNum * 6);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.6f);
        IsAnimationPlay = false;
    }

    //手紙を作成してポストから放出する(引数:stageNodesの順番、放出される順番)
    private void CreateLetter(int index,int releaseOrder)
    {
        SoundManager.PlaySE(SE_Enum.RELEASELETTER);
        Letter letter = Instantiate(letterPrefab,lettersTF);
        letters.Add(letter);
        letter.Initialize(stageNodes[index], this);
        letter.transform.localScale = initialzieLetterScale;
        letter.transform.localPosition = initialzieLetterPos;
        CalculateLetterOrbit(letter, releaseOrder);
    }

    //ポストから放出する手紙の軌道を計算し、DoTweenを作成する
    private void CalculateLetterOrbit(Letter letter, int releaseOrder)
    {
        var sequence = DOTween.Sequence();
        sequence
            .Append(letter.transform.DOLocalPath(
            path: letterOrbitArray[releaseOrder].path,
            duration: 0.6f,
            pathType:PathType.CatmullRom
            ).SetEase(Ease.InOutSine)).SetSpeedBased()
            .Join(letter.transform.DOScale(normalLetterScale, 0.6f).SetEase(Ease.InOutSine).SetSpeedBased());
        letter.SetStablePos(letterOrbitArray[releaseOrder].path[letterOrbitArray[releaseOrder].path.Length - 1]);
    }

    //表示するステージの変更を行う2つのボタンの表示・非表示の設定
    private void ButtonVisiableSetting()
    {
        //Downの設定
        if (indexNum <= 0)
        {
            //非表示
            downButton.IsValid = false;
        }
        else
        {
            //表示
            downButton.IsValid = true;
        }

        //Upの設定
        if(indexNum >= (stageNodes.Count - 1) / 6)
        {
            //非表示
            upButton.IsValid = false;
        }
        else
        {
            //表示
            upButton.IsValid = true;
        }

        StartCoroutine(ChangeShowStage());
    }

    //現在表示中のステージがどこかを説明するテキストの更新
    private void ExplainTextSetting()
    {
        explainNowShowStageText.text= indexNum * 6 + 1 + "-" + Mathf.Min((indexNum + 1) * 6, stageNodes.Count)
            + "件目表示中(全" + stageNodes.Count + "件)";
    }

    //表示するステージの変更
    private IEnumerator ChangeShowStage()
    {
        isAnimationPlay = true;
        foreach(Letter letter in letters)
        {
            Transform tf = letter.gameObject.transform;
            tf.DOLocalMove(new Vector3(tf.localPosition.x, 6.5f, tf.localPosition.z), 0.3f)
                .SetEase(Ease.InSine).OnComplete(
                ()=>
                {
                    StartCoroutine(DestroyLetter(letter));
                });
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.05f);
        yield return StartCoroutine(ReleaseLetters());
    }

    //手紙を一定時間後に破壊
    private IEnumerator DestroyLetter(Letter letter)
    {
        letter.transform.parent = this.transform;
        yield return new WaitForSeconds(0.6f);
        Destroy(letter.gameObject);
        letters.Remove(letter);

    }

    //ソートの種類の変更
    public void OnDropDownValueChange()
    {
        switch (dropDown.value)
        {
            case 0://新着順
                SortByDate();
                break;
            case 1://人気順
                SortByPopularity();
                break;
        }
        SoundManager.PlaySE(SE_Enum.DECIDE2);
        IndexNum = 0;
    }

    //新着順で並べ替え
    private void SortByDate()
    {
        stageNodes.Sort(SortByDateAuxiliary);
    }

    //新着順で並べ替えるための補助関数
    private int SortByDateAuxiliary(ExtraStageDataNode a, ExtraStageDataNode b)
    {
        if (a.CreatedTime == null)
        {
            return 1;
        }
        if (b.CreatedTime == null)
        {
            return -1;
        }
        return (b.CreatedTime).CompareTo(a.CreatedTime);
    }

    //人気順で並べ替え
    private void SortByPopularity()
    {
        stageNodes.Sort(SortByPopularityAuxiliary);
    }

    //人気順で並べ替えるための補助関数
    private int SortByPopularityAuxiliary(ExtraStageDataNode a, ExtraStageDataNode b)
    {
        return b.PlayNum - a.PlayNum;
    }

    //手紙を選択した時の処理
    public IEnumerator Selectletter(Letter letter)
    {
        IsAnimationPlay = true;
        isSelect = true;
        blackBack.SetActive(true);

        selectLetterNode = letter;
        selectLetter.Initialzie(letter.Data);
        selectLetterNode.GoSelectPos(new Vector3(0, -5.2f, 0), new Vector3(0.8f, 0.8f, 1), 0.2f);

        yield return new WaitForSeconds(0.25f);

        selectLetterNode.GetSpriteRenderer().enabled = false;
        yield return selectLetter.StartCoroutine(selectLetter.Show());
    }

    //選択されたステージを遊ぶ
    public void PlaySelectStage()
    {
        InformationDeliveryUnit.Instance.BlockEnums = selectLetterNode.Data.MapData;
        SceneChangeManager.GoGameFromExtraStageSelect(selectLetterNode.Data.ID);
    }

    //選択を解除
    public void CancelSelectStage()
    {
        StartCoroutine(CancelLetter());
    }

    //選択解除時のアニメーション
    private IEnumerator CancelLetter()
    {
        yield return selectLetter.StartCoroutine(selectLetter.Fold());

        selectLetterNode.GetSpriteRenderer().enabled = true;
        selectLetterNode.GoStablePos(0.2f);

        yield return new WaitForSeconds(0.2f);

        blackBack.SetActive(false);
        isSelect = false;
        IsAnimationPlay = false;
    }
}
