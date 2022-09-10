using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbyChild : AbstractChild
{
    [SerializeField] private SelectNode stageSelectNode;
    [SerializeField] private SelectNode extraStageSelectNode;
    [SerializeField] private SelectNode goTitleNode;
    [SerializeField] private GameObject boardNode;

    public override void ClickDeal(string nodeName)
    {
        if (nodeName == "StageSelectNode")
        {
            //ステージ選択画面に切り替える
            parent.SetState(SelectEnum.Lobby, SelectEnum.StageSelect);
        }

        if (nodeName == "GoTitleNode")
        {
            //タイトルに戻る
            SceneChangeManager.GoTitle(SceneEnum.Select);
        }

        if (nodeName == "ExtraStageNode")
        {
            //エクストラステージ選択画面に切り替える
            parent.SetState(SelectEnum.Lobby, SelectEnum.ExtraStageSelect);
        }
    }

    public override void Reset()
    {
        stageSelectNode.transform.localPosition = new Vector3(-13, -0.99f, 0);
        extraStageSelectNode.transform.localPosition = new Vector3(12.5f, 2.1f, 0);
        goTitleNode.transform.localPosition = new Vector3(11, -3.65f, 0);
        boardNode.transform.position = new Vector3(-4, 7, 0);
    }

    //タイトル画面から遷移してくる時の処理
    public void Initialize_FromTitle()
    {
        stageSelectNode.Initialize();
        goTitleNode.Initialize();
        extraStageSelectNode.Initialize();
        stageSelectNode.transform.localPosition = new Vector3(-3.22f, -0.99f, 0);
        extraStageSelectNode.transform.localPosition = new Vector3(4.75f, 2.1f, 0);
        goTitleNode.transform.localPosition = new Vector3(6.62f, -3.65f, 0);
        boardNode.transform.localPosition = new Vector3(-4, 4.3f, 0);

        parent.getStageNumbers().transform.parent = stageSelectNode.gameObject.transform;
        parent.getStageNumbers().transform.localPosition = new Vector3(-11.5f,2.05f, 0);
        parent.getStageNumbers().transform.localScale = new Vector3(1, 1, 1);
    }

    //ステージセレクト画面へと遷移する時の処理
    public void Finish_ToStageSelect()
    {
        TitleMove(7,0.2f);
        parent.getStageNumbers().transform.parent = parent.transform;
        stageSelectNode.gameObject.SetActive(false);
        SetMoveObj(goTitleNode, new Vector3(11, -3.65f, 0), 0,false);
        SetMoveObj(extraStageSelectNode, new Vector3(12.5f, 2.1f, 0), 0, false);
    }

    //ステージセレクト画面から遷移してくる時の処理
    public void Initialize_FromStageSelect()
    {
        TitleMove(4.3f,0.3f);
        stageSelectNode.Initialize();
        goTitleNode.Initialize();
        extraStageSelectNode.Initialize();
        Reset();
        Invoke("AppearStageSelectNode", 0.5f);
        SetMoveObj(goTitleNode, new Vector3(6.62f, -3.65f, 0), 0.5f,true);
        SetMoveObj(extraStageSelectNode, new Vector3(4.75f, 2.1f, 0), 0.5f, true);
    }

    //エクストラステージセレクト画面へと遷移する時の処理
    public void Finish_ToExtraStageSelect()
    {
        TitleMove(7, 0.2f);
        extraStageSelectNode.gameObject.SetActive(false);
        SetMoveObj(goTitleNode, new Vector3(11, -3.65f, 0), 0, false);
        SetMoveObj(stageSelectNode, new Vector3(-12.5f, -0.99f, 0), 0, false);
    }

    //エクストラステージセレクト画面から遷移してくる時の処理
    public void Initialize_FromExtraStageSelect()
    {
        TitleMove(4.3f, 0.3f);
        stageSelectNode.Initialize();
        goTitleNode.Initialize();
        extraStageSelectNode.Initialize();
        Reset();
        Invoke("AppearExtraStageSelectNode", 0.5f);
        SetMoveObj(goTitleNode, new Vector3(6.62f, -3.65f, 0), 0.5f, true);
        SetMoveObj(stageSelectNode, new Vector3(-3.22f, -0.99f, 0), 0.5f, true);

        parent.getStageNumbers().transform.parent = stageSelectNode.gameObject.transform;
        parent.getStageNumbers().transform.localPosition = new Vector3(-11.5f, 2.05f, 0);
        parent.getStageNumbers().transform.localScale = new Vector3(1, 1, 1);
    }

    private void AppearStageSelectNode()
    {
        stageSelectNode.transform.position = new Vector3(-3.22f, -0.99f, 0);
        stageSelectNode.SetIsValidTrue();

        parent.getStageNumbers().transform.parent = stageSelectNode.gameObject.transform;
        parent.getStageNumbers().transform.localPosition = new Vector3(-11.5f, 2.05f, 0);
        parent.getStageNumbers().transform.localScale = new Vector3(1, 1, 1);
    }

    private void AppearExtraStageSelectNode()
    {
        extraStageSelectNode.transform.position = new Vector3(4.75f, 2.1f, 0);
        extraStageSelectNode.SetIsValidTrue();
    }

    private void TitleMove(float toY,float delay)
    {
        boardNode.transform.DOLocalMove(new Vector3(-4, toY, 0),0.5f).SetDelay(delay);
    }

    public void SetObjIsValidTrue()
    {
        stageSelectNode.SetIsValidTrue();
        extraStageSelectNode.SetIsValidTrue();
        goTitleNode.SetIsValidTrue();
    }
}
