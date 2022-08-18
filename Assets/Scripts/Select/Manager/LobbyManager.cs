using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbyManager : AbstractManager
{
    [SerializeField] private SelectNode stageSelectNode;
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
    }

    public override void Reset()
    {
        stageSelectNode.transform.position = new Vector3(-13, -0.99f, 0);
        goTitleNode.transform.position = new Vector3(11, -3.65f, 0);
        boardNode.transform.position = new Vector3(-4, 7, 0);
    }

    public void Initialize_FromTitle()
    {
        stageSelectNode.Initialize();
        goTitleNode.Initialize();
        stageSelectNode.transform.position = new Vector3(-3.22f, -0.99f, 0);
        goTitleNode.transform.position = new Vector3(6.62f, -3.65f, 0);
        boardNode.transform.position = new Vector3(-4, 4.3f, 0);

        parent.getStageNumbers().transform.parent = stageSelectNode.gameObject.transform;
        parent.getStageNumbers().transform.localPosition = new Vector3(-11.5f,2.05f, 0);
    }

    public void SetObjIsValidTrue()
    {
        stageSelectNode.SetIsValidTrue();
        goTitleNode.SetIsValidTrue();
    }

    public void Finish_ToStageSelect()
    {
        TitleMove(7,0.2f);
        parent.getStageNumbers().transform.parent = parent.transform;
        stageSelectNode.gameObject.SetActive(false);
        SetMoveObj(goTitleNode, new Vector3(11, -3.65f, 0), 0,false);
    }

    public void Initialize_FromStageSelect()
    {
        TitleMove(4.3f,0.3f);
        stageSelectNode.Initialize();
        goTitleNode.Initialize();
        Reset();
        Invoke("AppearStageSelectNode", 0.5f);
        SetMoveObj(goTitleNode, new Vector3(6.62f, -3.65f, 0), 0.5f,true);
    }

    private void AppearStageSelectNode()
    {
        stageSelectNode.transform.position = new Vector3(-3.22f, -0.99f, 0);
        stageSelectNode.SetIsValidTrue();

        parent.getStageNumbers().transform.parent = stageSelectNode.gameObject.transform;
        parent.getStageNumbers().transform.localPosition = new Vector3(-11.5f, 2.05f, 0);
        parent.getStageNumbers().transform.localScale = new Vector3(1, 1, 1);
    }

    private void TitleMove(float toY,float delay)
    {
        boardNode.transform.DOLocalMove(new Vector3(-4, toY, 0),0.5f).SetDelay(delay);
    }
}
