using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    [SerializeField] private LobbyManager lobby;
    [SerializeField] private StageSelectManager stageSelect;
    [SerializeField] private GameObject stageNumbers;
    private bool isValid;
    public bool IsValid
    {
        get { return isValid; }
        set { isValid = value; }
    }

    public GameObject getStageNumbers()
    {
        return stageNumbers;
    }

    private SelectEnum nowState;

    //初期化
    public void Initialize()
    {
        stageSelect.Reset();
        lobby.Reset();

        SetState(SelectEnum.None, SelectEnum.Lobby);
    }

    //暗転解除後の処理
    public void AfterOpenCurtainDeal()
    {
        lobby.SetObjIsValidTrue();
        isValid = true;
    }

    //状態の変化
    public void SetState(SelectEnum from,SelectEnum to)
    {
        if (from.Equals(SelectEnum.None))
        {
            //タイトル画面からLobbyへ
            nowState = SelectEnum.Lobby;
            lobby.Initialize_FromTitle();
        }

        if (to.Equals(SelectEnum.StageSelect))
        {
            //lobbyからstageSelectへ
            nowState = SelectEnum.StageSelect;
            lobby.Finish_ToStageSelect();
            stageSelect.Initialize_FromLobby();
        }

        if (from.Equals(SelectEnum.StageSelect) && to.Equals(SelectEnum.Lobby))
        {
            //stageSelectからlobbyへ
            nowState = SelectEnum.Lobby;
            stageSelect.Finish_ToLobby();
            lobby.Initialize_FromStageSelect();
        }
    }
}
