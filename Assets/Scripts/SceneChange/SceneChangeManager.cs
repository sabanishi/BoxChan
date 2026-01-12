using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager instance { get; private set; }
    [SerializeField] private CurtainManager _curtainManager;
    [SerializeField] private TitleScene titleScene;
    [SerializeField] private SelectScene seletScene;
    [SerializeField] private GameScene gameScene;
    [SerializeField] private CreateStageScene createStageScene;
    [SerializeField] private ExtraStageSelectScene extraStageSelectScene;
    [SerializeField] private CreditScene creditScene;

    private bool isChangeing;

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
        GoTitle(SceneEnum.None);
    }

    public static void GoTitle(SceneEnum from)
    {
        if (instance.isChangeing) return;

        if (from == SceneEnum.Select)
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.seletScene, instance.titleScene,""));
        }
        else
        {
            SoundManager.PlayBGM(BGM_Enum.TITLE);
            instance.StartCoroutine(instance.GoStageCoroutine(null, instance.titleScene,""));
        }
    }

    public static void GoSelect(SceneEnum fromScene)
    {
        if (instance.isChangeing) return;

        if (fromScene.Equals(SceneEnum.Title))
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.titleScene, instance.seletScene,"Title"));
        }else if (fromScene.Equals(SceneEnum.Game))
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.gameScene, instance.seletScene,"Game"));
        }
        else if (fromScene.Equals(SceneEnum.CreateStage))
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.createStageScene, instance.seletScene, "CreateStage"));
        }
        else if (fromScene.Equals(SceneEnum.ExtraStageSelect))
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.extraStageSelectScene, instance.seletScene, "ExtraStageSelect"));
        }
        else if (fromScene.Equals(SceneEnum.Credit))
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.creditScene, instance.seletScene, "Title"));
        }
        else
        {
            Debug.Log(fromScene + "が用意されていない");
        }
    }

    public static void GoGameFromExtraStage(string stageName)
    {
        if (instance.isChangeing) return;
        instance.StartCoroutine(instance.GoStageCoroutine(instance.seletScene, instance.gameScene,stageName));
    }

    public static void GoGameFromCreateStage()
    {
        if (instance.isChangeing) return;
        instance.StartCoroutine(instance.GoStageCoroutine(instance.createStageScene, instance.gameScene,"CreateStage"));
    }

    public static void GoGameFromExtraStageSelect(string stageName)
    {
        if (instance.isChangeing) return;
        instance.StartCoroutine(instance.GoStageCoroutine(instance.extraStageSelectScene, instance.gameScene, stageName));
    }

    public static void GoCreateStage(SceneEnum fromScene,bool isClear)
    {
        if (instance.isChangeing) return;
        if (fromScene.Equals(SceneEnum.Select))
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.seletScene, instance.createStageScene, "Select"));
        }
        else
        {
            if (isClear)
            {
                instance.StartCoroutine(instance.GoStageCoroutine(instance.gameScene, instance.createStageScene, "GameClear"));
            }
            else
            {
                instance.StartCoroutine(instance.GoStageCoroutine(instance.gameScene, instance.createStageScene, "GameNotClear"));
            }
        }
    }

    public static void GoExtraStageSelect(SceneEnum fromScene)
    {
        if (instance.isChangeing) return;
        if (fromScene.Equals(SceneEnum.Select))
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.seletScene, instance.extraStageSelectScene, "StageSelect"));
        }
        else if(fromScene.Equals(SceneEnum.Game))
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.gameScene, instance.extraStageSelectScene, "Game"));
        }
    }

    public static void GoCredit()
    {
        instance.StartCoroutine(instance.GoStageCoroutine(instance.seletScene, instance.creditScene, ""));
    }

    private IEnumerator GoStageCoroutine(SceneChangeAbstract from,SceneChangeAbstract to,string initialize_value)
    {
        isChangeing = true;

        //エクストラステージセレクトに入り、かつまだエクストラステージのデータを一度も読み込んでいない時、読み込みを開始する
        if (to.Equals(instance.extraStageSelectScene)&&!DatabaseConnector.Instance.IsAlreadyLoad)
        {
            DatabaseConnector.FetchList();
        }

        //入力受け付け停止
        if (from != null)
        {
            from.BeforeCloseCurtainDeal();
            //暗転
            yield return StartCoroutine(CloseCurtainCoroutine());
            if (initialize_value.Equals("Game")||(initialize_value.Equals("CreateStage")&&to.Equals(instance.seletScene))
                ||initialize_value.Equals("Select"))
            {
                SoundManager.StopBGM();
            }
        }

        //場面転換
        if (from != null)
        {
            if (to.Equals(instance.gameScene) && !from.Equals(instance.createStageScene))
            {
                SoundManager.StopBGM();
            }
            from.Terminate();
            from.gameObject.SetActive(false);
        }
        to.gameObject.SetActive(true);
        to.Initialize(initialize_value);

        if (to.Equals(instance.extraStageSelectScene))
        {
            while (!instance.extraStageSelectScene.IsLoadFinish)
            {
                yield return null;
            }
        }
        
        if (from != null)
        {
            //暗転解除
            yield return StartCoroutine(OpenCurtainCoroutine());
            if (to.Equals(instance.seletScene)||to.Equals(instance.extraStageSelectScene))
            {
                SoundManager.PlayBGM(BGM_Enum.TITLE);
            }
            if (initialize_value.Equals("GameClear") || initialize_value.Equals("GameNotClear")|| initialize_value.Equals("Select"))
            {
                SoundManager.PlayBGM(BGM_Enum.CREATESTAGE);
            }
        }
        to.AfterOpenCurtainDeal();

        isChangeing = false;
    }


    //荷物の山を上から降らせる
    public static IEnumerator CloseCurtainCoroutine()
    {
        yield return instance._curtainManager.StartCoroutine(instance._curtainManager.AppearLetterCoroutine());
    }

    //荷物の山を下に落とす
    public static IEnumerator OpenCurtainCoroutine()
    {
        yield return instance._curtainManager.StartCoroutine(instance._curtainManager.DisappearLetterCoroutine());
    }
}
