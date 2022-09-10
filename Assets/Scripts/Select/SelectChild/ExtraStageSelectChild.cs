using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExtraStageSelectChild : AbstractChild
{
    [SerializeField] private GameObject ExtraStageSelectNode;
    [SerializeField] private SelectNode CreateNode;
    [SerializeField] private SelectNode PlayNode;
    [SerializeField] private SelectNode GoBack;

    public override void ClickDeal(string nodeName)
    {
        if (nodeName == "GoBackNode")
        {
            parent.SetState(SelectEnum.ExtraStageSelect, SelectEnum.Lobby);
        }

        if (nodeName == "PlayNode")
        {
            parent.IsValid = false;
            SceneChangeManager.GoExtraStageSelect(SceneEnum.Select);
        }

        if(nodeName=="CreateNode")
        {
            parent.IsValid = false;
            SceneChangeManager.GoCreateStage(SceneEnum.Select,false);
        }
    }

    public override void Reset()
    {
        ExtraStageSelectNode.transform.localPosition = new Vector3(15, 2, 0);
        GoBack.transform.position = new Vector3(11, -3.65f, 0);
    }

    public void Initialize_FromCreateStage()
    {
        ExtraStageSelectNode.transform.localPosition = new Vector3(-0.25f, 0.15f, 0);
        ExtraStageSelectNode.transform.localScale = new Vector3(0.27f, 0.27f, 1);
        CreateNode.Initialize();
        PlayNode.Initialize();
        CreateNode.SetIsValidTrue();
        PlayNode.SetIsValidTrue();

        GoBack.Initialize();
        GoBack.SetIsValidTrue();
        GoBack.transform.localPosition = new Vector3(6.62f, -3.65f, 0);
    }

    public void Initialize_FromLobby()
    {
        ExtraStageSelectNode.transform.localPosition = new Vector3(4.75f, 2.1f, 0);
        ExtraStageSelectNode.transform.localScale = new Vector3(0.18f, 0.18f, 1);
        CreateNode.Initialize();
        PlayNode.Initialize();
        InitializeMoveObj();

        GoBack.Initialize();
        GoBack.transform.position = new Vector3(11, -3.65f, 0);
        SetMoveObj(GoBack, new Vector3(6.62f, -3.65f, 0), 0.6f, true);
    }

    public void Finish_ToLobby()
    {
        FinishMoveCard();
        CreateNode.SetIsvalidFalse();
        PlayNode.SetIsvalidFalse();
        SetMoveObj(GoBack, new Vector3(11, -3.65f, 0), 0, false);
    }

    private void InitializeMoveObj()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(ExtraStageSelectNode.transform.DOLocalMove(new Vector3(-0.25f, 0.15f, 0), 0.2f)
            .SetDelay(0.2f).OnComplete(FinishMove));
        sequence.Join(ExtraStageSelectNode.transform.DOScale(new Vector3(0.27f, 0.27f, 1), 0.2f));
        sequence.Play();
    }

    private void FinishMoveCard()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(ExtraStageSelectNode.transform.DOLocalMove(new Vector3(4.75f, 2.1f, 0), 0.2f)
            .SetDelay(0.2f).OnComplete(() => { Invoke("DissapearObj", 0.1f); }));
        sequence.Join(ExtraStageSelectNode.transform.DOScale(new Vector3(0.16f, 0.16f, 1), 0.2f));
        sequence.Play();
    }

    private void DissapearObj()
    {
        ExtraStageSelectNode.transform.localPosition = new Vector3(15, 2, 0);
    }

    private void FinishMove()
    {
        CreateNode.SetIsValidTrue();
        PlayNode.SetIsValidTrue();
    }
}
