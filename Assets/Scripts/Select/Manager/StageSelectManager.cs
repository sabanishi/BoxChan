using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StageSelectManager : AbstractManager
{
    [SerializeField] private GameObject CardObj;
    [SerializeField] private SelectNode GoBack;
    [SerializeField] private GameObject InfoBack;

    [SerializeField] private Text DayText;
    [SerializeField] private Text TimeText;
    [SerializeField] private Text RankText;
    [SerializeField] private Text WorlRecordText;
    private int nowShowInfoNum;

    private bool isValidStageNum;
    public bool IsValidStageNum()
    {
        return isValidStageNum;
    }

    private bool isMove;

    public override void ClickDeal(string nodeName)
    {
        if (nodeName == "GoBackNode")
        {
            parent.SetState(SelectEnum.StageSelect, SelectEnum.Lobby);
        }
    }

    public override void Reset()
    {
        CardObj.transform.position = new Vector3(-13, -0.05f, 0);
        CardObj.transform.localScale = new Vector3(0.33f, 0.33f, 1);
        GoBack.transform.position = new Vector3(11, -3.65f, 0);
        InfoBack.SetActive(false);
    }

    public void Initialize_FromLobby()
    {
        isValidStageNum = false;
        CardObj.transform.position = new Vector3(-3.22f, -0.99f, 0);
        CardObj.transform.localScale = new Vector3(0.33f, 0.33f, 1);
        InitilizeMoveCard();

        GoBack.Initialize();
        GoBack.transform.position = new Vector3(11, -3.65f, 0);
        SetMoveObj(GoBack, new Vector3(6.62f, -3.65f, 0), 0.6f,true);

        parent.getStageNumbers().transform.parent = CardObj.transform;
        parent.getStageNumbers().transform.localPosition = new Vector3(-11.5f, 2.05f, 0);
        parent.getStageNumbers().transform.localScale = new Vector3(1, 1, 1);
    }

    public void Initialize_FromGame()
    {
        
    }

    private void InitilizeMoveCard()
    {
        isMove = true;
        var sequence =DOTween.Sequence();
        sequence.Append(CardObj.transform.DOLocalMove(new Vector3(-1.105f, 0.05f, 0),0.2f).SetDelay(0.2f).OnComplete(AppearNode));
        sequence.Join(CardObj.transform.DOScale(new Vector3(0.45f,0.45f,1),0.2f));
        sequence.Play();
    }

    //ステージ番号の表示
    private void AppearNode()
    {
        isValidStageNum = true;
        isMove = false;
    }

    public void Finish_ToLobby()
    {
        isValidStageNum = false;
        FinishMoveCard();
        SetMoveObj(GoBack, new Vector3(11, -3.65f, 0), 0,false);
    }

    private void FinishMoveCard()
    {
        var sequence =DOTween.Sequence();
        sequence.Append(CardObj.transform.DOLocalMove(new Vector3(-3.22f,-0.99f, 0),0.2f).SetDelay(0.2f).OnComplete(DissapearCard));
        sequence.Join(CardObj.transform.DOScale(new Vector3(0.3f,0.3f,1),0.2f));
        sequence.Play();
    }

    public void DissapearCard()
    {
        DissapearStageInfo();
        Invoke("SetCardFalse", 0.2f);
    }

    private void SetCardFalse()
    {
        if (isMove) return;
        CardObj.transform.position = new Vector3(-13f, -0.99f, 0);
    }

    public void ClickNumber(int num)
    {
        SceneChangeManager.GoGame(num);
    }

    //ステージ情報の表示
    public void DisplayStageInfo(int num)
    {
        DayText.text = "Day" + num;
        TimeText.text = "100:45:20";
        RankText.text = "32 位";
        WorlRecordText.text = "90:64:21";
        nowShowInfoNum = num;
        InfoBack.SetActive(true);
    }

    //ステージ情報の非表示
    public void DissapearStageInfo(int num)
    {
        if (num == nowShowInfoNum)
        {
            DissapearStageInfo();
            InfoBack.SetActive(false);
        }
    }

    //ステージ情報を消す
    private void DissapearStageInfo()
    {
        DayText.text = "";
        TimeText.text = "";
        RankText.text = "";
        WorlRecordText.text = "";
        nowShowInfoNum = 0;
    }
}
