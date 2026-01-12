using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    [SerializeField] private LobbyChild lobby;
    [SerializeField] private StageSelectChild stageSelect;
    [SerializeField] private ExtraStageSelectChild extraStageSelect;
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

    //遊べるステージの最大値
    private int canPlayStageMaxNum;

    //初期化
    public void Initialize(string initialize_value)
    {
        stageSelect.Reset();
        lobby.Reset();
        extraStageSelect.Reset();

        if (initialize_value.Equals("Title"))
        {
            //fromタイトル
            SetState(SelectEnum.None, SelectEnum.Lobby);
        }
        else if (initialize_value.Equals("Game"))
        {
            //fromゲーム
            SetState(SelectEnum.None, SelectEnum.StageSelect);
        }else if (initialize_value.Equals("CreateStage")||initialize_value.Equals("ExtraStageSelect"))
        {
            //fromステージ制作
            SetState(SelectEnum.None, SelectEnum.ExtraStageSelect);
        }
    }

    //暗転解除後の処理
    public void AfterOpenCurtainDeal()
    {
        lobby.SetObjIsValidTrue();
        isValid = true;
    }

    //終わり時の処理
    public void FinishDeal()
    {
        stageSelect.Dissapear();
    }

    //状態の変化
    public void SetState(SelectEnum from,SelectEnum to)
    {
        if (from.Equals(SelectEnum.None))
        {
            if (to.Equals(SelectEnum.Lobby))
            {
                //タイトル画面からLobbyへ
                lobby.Initialize_FromTitle();
            }
            else if(to.Equals(SelectEnum.StageSelect))
            {
                //ゲーム画面からStageSelectへ
                stageSelect.Initialize_FromGame();
            }else if (to.Equals(SelectEnum.ExtraStageSelect))
            {
                //ステージ制作画面からExtraStageelectへ
                extraStageSelect.Initialize_FromCreateStage();
            }
        }
        else if (from.Equals(SelectEnum.Lobby))
        {
            if (to.Equals(SelectEnum.StageSelect))
            {
                //lobbyからstageSelectへ
                lobby.Finish_ToStageSelect();
                stageSelect.Initialize_FromLobby();
            }
            else if (to.Equals(SelectEnum.ExtraStageSelect))
            {
                //lobbyからextraSelectへ
                lobby.Finish_ToExtraStageSelect();
                extraStageSelect.Initialize_FromLobby();
            }

        }
        else if(from.Equals(SelectEnum.StageSelect))
        {
            if (to.Equals(SelectEnum.Lobby))
            {
                //stageSelectからlobbyへ
                stageSelect.Finish_ToLobby();
                lobby.Initialize_FromStageSelect();
            }
        }
        else if (from.Equals(SelectEnum.ExtraStageSelect))
        {
            if (to.Equals(SelectEnum.Lobby))
            {
                //extraSelectからlobbyへ
                extraStageSelect.Finish_ToLobby();
                lobby.Initialize_FromExtraStageSelect();
            }
        }
    }
}
